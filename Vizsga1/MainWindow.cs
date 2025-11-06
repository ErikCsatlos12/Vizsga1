using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using UMFST.MIP.Bookstore.Models;
using System.IO;
using Microsoft.Data.Sqlite; // A Microsoft ADO.NET csomagja

namespace UMFST.MIP.Bookstore
{
    public partial class MainWindow : Form
    {
        // --- VÁLTOZÓK ---
        private const string JsonDataUrl = "https://cdn.shopify.com/s/files/1/0883/3282/8936/files/data_bookstore_final.json?v=1762418524";
        private const string ErrorLogFile = "invalid_bookstore.txt";
        private const string ReportFile = "sales_report.txt"; // A README által kért fájlnév
        private static string dbFile = "bookstore.sqlite";
        private static string connectionString = $"Data Source={dbFile}";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Indításkor csak inicializálunk
        }

        // --- FŐ GOMBOK ESEMÉNYKEZELŐI ---

        private async void btnResetDatabase_Click(object sender, EventArgs e)
        {
            var valasz = MessageBox.Show("Biztosan törölni akarja a teljes adatbázist és újraimportálni az adatokat?",
                "Adatbázis Reset Megerősítése", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (valasz == DialogResult.No) return;
            await ResetAndImportDataAsync();
        }

        private async void btnImportData_Click(object sender, EventArgs e)
        {
            var valasz = MessageBox.Show("Újra letölti és feldolgozza az adatokat a linkről?",
                "Import Megerősítése", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (valasz == DialogResult.No) return;
            await ResetAndImportDataAsync();
        }

        // --- FŐ LOGIKAI METÓDUSOK (BŐVÍTETT IMPORTÁLÁSSAL) ---

        private async Task ResetAndImportDataAsync()
        {
            btnImportData.Enabled = false;
            btnResetDatabase.Enabled = false;
            lblStatus.Text = "Folyamat elindítva...";

            try
            {
                lblStatus.Text = "Adatbázis törlése...";
                await DatabaseService.ResetDatabaseAsync();
                lblStatus.Text = "Adatbázis sikeresen létrehozva.";

                if (File.Exists(ErrorLogFile))
                    File.Delete(ErrorLogFile);

                lblStatus.Text = "JSON letöltése a szerverről...";
                string jsonString;
                using (HttpClient client = new HttpClient())
                {
                    jsonString = await client.GetStringAsync(JsonDataUrl);
                }
                if (string.IsNullOrEmpty(jsonString))
                {
                    throw new Exception("A letöltött JSON üres.");
                }

                lblStatus.Text = "JSON feldolgozása...";
                RootData data = JsonConvert.DeserializeObject<RootData>(jsonString);

                lblStatus.Text = "Adatok importálása az adatbázisba...";

                using (var conn = new SqliteConnection(connectionString))
                using (StreamWriter log = new StreamWriter(ErrorLogFile, append: true))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction())
                    {
                        await log.WriteLineAsync($"--- Import Napló ({DateTime.Now}) ---");

                        // --- Szerzők (Authors) importálása ---
                        if (data.Authors != null)
                        {
                            var cmd = new SqliteCommand("INSERT INTO Authors (Id, Name, Country) VALUES (@Id, @Name, @Country)", conn, transaction);
                            foreach (var author in data.Authors)
                            {
                                cmd.Parameters.Clear();
                                DatabaseService.AddParameter(cmd, "@Id", author.Id);
                                DatabaseService.AddParameter(cmd, "@Name", author.Name);
                                DatabaseService.AddParameter(cmd, "@Country", author.Country);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        // --- Könyvek (Books) importálása VALIDÁLÁSSAL ---
                        var validBookIsbns = new HashSet<string>();
                        if (data.Books != null)
                        {
                            var cmd = new SqliteCommand("INSERT INTO Books (Isbn, Title, Price, Stock, AuthorId) VALUES (@Isbn, @Title, @Price, @Stock, @AuthorId)", conn, transaction);
                            foreach (var book in data.Books)
                            {
                                if (string.IsNullOrEmpty(book.Isbn) || book.Price < 0 || book.Stock < 0)
                                {
                                    await log.WriteLineAsync($"[HIBA-KÖNYV] Kihagyva: ISBN='{book.Isbn}', Cím='{book.Title}', Ár={book.Price}, Készlet={book.Stock}");
                                }
                                else
                                {
                                    cmd.Parameters.Clear();
                                    DatabaseService.AddParameter(cmd, "@Isbn", book.Isbn);
                                    DatabaseService.AddParameter(cmd, "@Title", book.Title);
                                    DatabaseService.AddParameter(cmd, "@Price", book.Price);
                                    DatabaseService.AddParameter(cmd, "@Stock", book.Stock);
                                    DatabaseService.AddParameter(cmd, "@AuthorId", book.AuthorId);
                                    await cmd.ExecuteNonQueryAsync();
                                    validBookIsbns.Add(book.Isbn);
                                }
                            }
                        }

                        // --- Fizetések (Payments) importálása ---
                        if (data.Payments != null)
                        {
                            var cmd = new SqliteCommand("INSERT INTO Payments (Id, OrderId, Method, Amount, Captured) VALUES (@Id, @OrderId, @Method, @Amount, @Captured)", conn, transaction);
                            foreach (var payment in data.Payments)
                            {
                                cmd.Parameters.Clear();
                                DatabaseService.AddParameter(cmd, "@Id", payment.Id);
                                DatabaseService.AddParameter(cmd, "@OrderId", payment.OrderId);
                                DatabaseService.AddParameter(cmd, "@Method", payment.Method);
                                DatabaseService.AddParameter(cmd, "@Amount", payment.Amount);
                                DatabaseService.AddParameter(cmd, "@Captured", payment.Captured);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        // --- Vevők (Customers), Rendelések (Orders) és Tételek (OrderItems) importálása ---
                        if (data.Orders != null)
                        {
                            var customersCmd = new SqliteCommand("INSERT OR IGNORE INTO Customers (Id, Name, Email) VALUES (@Id, @Name, @Email)", conn, transaction);
                            var ordersCmd = new SqliteCommand("INSERT INTO Orders (Id, DateString, Date, Status, CustomerId) VALUES (@Id, @DateString, @Date, @Status, @CustomerId)", conn, transaction);
                            var itemsCmd = new SqliteCommand("INSERT INTO OrderItems (Qty, UnitPrice, Discount, OrderId, BookIsbn) VALUES (@Qty, @UnitPrice, @Discount, @OrderId, @BookIsbn)", conn, transaction);

                            foreach (var order in data.Orders)
                            {
                                bool validOrder = true;

                                if (order.CustomerJson == null || string.IsNullOrEmpty(order.CustomerJson.Id) || !order.CustomerJson.Email.Contains("@"))
                                {
                                    await log.WriteLineAsync($"[HIBA-CUSTOMER] Kihagyva: Rendelés ID='{order.Id}', Customer hiba.");
                                    validOrder = false;
                                }
                                else
                                {
                                    customersCmd.Parameters.Clear();
                                    DatabaseService.AddParameter(customersCmd, "@Id", order.CustomerJson.Id);
                                    DatabaseService.AddParameter(customersCmd, "@Name", order.CustomerJson.Name);
                                    DatabaseService.AddParameter(customersCmd, "@Email", order.CustomerJson.Email);
                                    await customersCmd.ExecuteNonQueryAsync();
                                    order.CustomerId = order.CustomerJson.Id;
                                }

                                if (!DateTime.TryParse(order.DateString, out DateTime parsedDate))
                                {
                                    await log.WriteLineAsync($"[HIBA-DÁTUM] Kihagyva: Rendelés ID='{order.Id}', Dátum='{order.DateString}'");
                                    validOrder = false;
                                }
                                else
                                {
                                    order.Date = parsedDate;
                                }

                                if (!validOrder) continue;
                                ordersCmd.Parameters.Clear();
                                DatabaseService.AddParameter(ordersCmd, "@Id", order.Id);
                                DatabaseService.AddParameter(ordersCmd, "@DateString", order.DateString);
                                DatabaseService.AddParameter(ordersCmd, "@Date", order.Date);
                                DatabaseService.AddParameter(ordersCmd, "@Status", order.Status);
                                DatabaseService.AddParameter(ordersCmd, "@CustomerId", order.CustomerId);
                                await ordersCmd.ExecuteNonQueryAsync();

                                if (order.Items != null)
                                {
                                    foreach (var item in order.Items)
                                    {
                                        if (item.Qty <= 0 || !validBookIsbns.Contains(item.BookIsbn))
                                        {
                                            await log.WriteLineAsync($"[HIBA-TÉTEL] Kihagyva: Rendelés ID='{order.Id}', ISBN='{item.BookIsbn}', Qty={item.Qty}");
                                        }
                                        else
                                        {
                                            itemsCmd.Parameters.Clear();
                                            DatabaseService.AddParameter(itemsCmd, "@Qty", item.Qty);
                                            DatabaseService.AddParameter(itemsCmd, "@UnitPrice", item.UnitPrice);
                                            DatabaseService.AddParameter(itemsCmd, "@Discount", item.Discount);
                                            DatabaseService.AddParameter(itemsCmd, "@OrderId", order.Id);
                                            DatabaseService.AddParameter(itemsCmd, "@BookIsbn", item.BookIsbn);
                                            await itemsCmd.ExecuteNonQueryAsync();
                                        }
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                    }
                }

                // --- NÉZET FRISSÍTÉSE ---
                lblStatus.Text = "Nézetek frissítése...";
                await LoadBooksAsync();
                await LoadOrdersAsync();
                await GenerateReportAsync(); // <-- ÚJ HÍVÁS: A riportot is legeneráljuk

                lblStatus.Text = "Importálás sikeresen befejezve!";
                MessageBox.Show("Az adatbázis sikeresen törölve és újraimportálva a JSON alapján!", "Import Kész", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Kritikus hiba történt!";
                MessageBox.Show($"Hiba történt a folyamat során: {ex.Message}\n\nRészletek: {ex.StackTrace}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnImportData.Enabled = true;
                btnResetDatabase.Enabled = true;
            }
        }

        // --- "BOOKS" FÜL METÓDUSAI ---
        // ... (A LoadBooksAsync, btnSearchBooks_Click, btnRestock_Click metódusok itt vannak, ahogy az előző lépésben írtuk) ...
        #region Books Tab Logic
        private async Task LoadBooksAsync()
        {
            lblStatus.Text = "Könyvek betöltése...";
            try
            {
                string sql = @"
                    SELECT b.Isbn, b.Title, a.Name AS AuthorName, b.Price, b.Stock
                    FROM Books b
                    LEFT JOIN Authors a ON b.AuthorId = a.Id";

                using (var conn = new SqliteConnection(connectionString))
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    var dataTable = new DataTable();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                    gridBooks.DataSource = dataTable;
                }

                if (gridBooks.Columns["Isbn"] != null)
                    gridBooks.Columns["Isbn"].HeaderText = "ISBN";
                if (gridBooks.Columns["Title"] != null)
                    gridBooks.Columns["Title"].HeaderText = "Cím";
                if (gridBooks.Columns["AuthorName"] != null)
                    gridBooks.Columns["AuthorName"].HeaderText = "Szerző";
                if (gridBooks.Columns["Price"] != null)
                    gridBooks.Columns["Price"].HeaderText = "Ár (EUR)";
                if (gridBooks.Columns["Stock"] != null)
                    gridBooks.Columns["Stock"].HeaderText = "Készlet (db)";

                lblStatus.Text = "Könyvek betöltve.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a könyvek betöltésekor: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Hiba a könyvek betöltésekor.";
            }
        }

        private async void btnSearchBooks_Click(object sender, EventArgs e)
        {
            string searchTerm = $"%{txtSearchBooks.Text.Trim()}%";
            lblStatus.Text = "Keresés...";

            try
            {
                string sql = @"
                    SELECT b.Isbn, b.Title, a.Name AS AuthorName, b.Price, b.Stock
                    FROM Books b
                    LEFT JOIN Authors a ON b.AuthorId = a.Id
                    WHERE b.Title LIKE @SearchTerm OR a.Name LIKE @SearchTerm";

                using (var conn = new SqliteConnection(connectionString))
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    DatabaseService.AddParameter(cmd, "@SearchTerm", searchTerm);
                    await conn.OpenAsync();
                    var dataTable = new DataTable();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                    gridBooks.DataSource = dataTable;
                    lblStatus.Text = $"{dataTable.Rows.Count} találat.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a keresés során: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRestock_Click(object sender, EventArgs e)
        {
            if (gridBooks.CurrentRow == null)
            {
                MessageBox.Show("Nincs kijelölt könyv a készlet növeléséhez!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string isbn = gridBooks.CurrentRow.Cells["Isbn"].Value.ToString();
            string title = gridBooks.CurrentRow.Cells["Title"].Value.ToString();

            var valasz = MessageBox.Show($"Biztosan növeli 10-zel a '{title}' könyv készletét?", "Megerősítés", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (valasz == DialogResult.No) return;

            try
            {
                string sql = "UPDATE Books SET Stock = Stock + 10 WHERE Isbn = @Isbn";

                using (var conn = new SqliteConnection(connectionString))
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    DatabaseService.AddParameter(cmd, "@Isbn", isbn);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }

                lblStatus.Text = $"'{title}' készlete növelve.";
                btnSearchBooks_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a készlet frissítésekor: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        // --- "ORDERS" FÜL METÓDUSAI ---
        // ... (A LoadOrdersAsync, gridOrders_SelectionChanged metódusok itt vannak) ...
        #region Orders Tab Logic
        private async Task LoadOrdersAsync()
        {
            lblStatus.Text = "Rendelések betöltése...";
            try
            {
                string sql = @"
                    SELECT o.Id, c.Name AS CustomerName, o.Date, o.Status
                    FROM Orders o
                    LEFT JOIN Customers c ON o.CustomerId = c.Id
                    ORDER BY o.Date DESC";

                using (var conn = new SqliteConnection(connectionString))
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    var dataTable = new DataTable();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                    gridOrders.DataSource = dataTable;
                }

                if (gridOrders.Columns["Id"] != null)
                    gridOrders.Columns["Id"].HeaderText = "Rendelés ID";
                if (gridOrders.Columns["CustomerName"] != null)
                    gridOrders.Columns["CustomerName"].HeaderText = "Vevő Neve";
                if (gridOrders.Columns["Date"] != null)
                    gridOrders.Columns["Date"].HeaderText = "Dátum";
                if (gridOrders.Columns["Status"] != null)
                    gridOrders.Columns["Status"].HeaderText = "Állapot";

                lblStatus.Text = "Rendelések betöltve.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a rendelések betöltésekor: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Hiba a rendelések betöltésekor.";
            }
        }

        private async void gridOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (gridOrders.CurrentRow == null)
            {
                gridOrderItems.DataSource = null;
                lblOrderTotal.Text = "Végösszeg: 0 EUR";
                return;
            }

            string orderId = gridOrders.CurrentRow.Cells["Id"].Value.ToString();
            string status = gridOrders.CurrentRow.Cells["Status"].Value.ToString();

            // Kiemelés a README szerint
            if (status == "PENDING" || status == "INVALID")
            {
                gridOrders.CurrentRow.DefaultCellStyle.BackColor = Color.LightCoral;
            }
            else
            {
                gridOrders.CurrentRow.DefaultCellStyle.BackColor = Color.White;
            }

            try
            {
                string sql = @"
                    SELECT 
                        b.Title AS BookTitle,
                        oi.Qty,
                        oi.UnitPrice,
                        oi.Discount,
                        (oi.Qty * oi.UnitPrice - IFNULL(oi.Discount, 0)) AS Total
                    FROM OrderItems oi
                    JOIN Books b ON oi.BookIsbn = b.Isbn
                    WHERE oi.OrderId = @OrderId";

                decimal orderTotal = 0;

                using (var conn = new SqliteConnection(connectionString))
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    DatabaseService.AddParameter(cmd, "@OrderId", orderId);
                    await conn.OpenAsync();

                    var dataTable = new DataTable();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }

                    gridOrderItems.DataSource = dataTable;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        orderTotal += Convert.ToDecimal(row["Total"]);
                    }
                }

                if (gridOrderItems.Columns["BookTitle"] != null)
                    gridOrderItems.Columns["BookTitle"].HeaderText = "Könyv Címe";
                if (gridOrderItems.Columns["Qty"] != null)
                    gridOrderItems.Columns["Qty"].HeaderText = "Db";
                if (gridOrderItems.Columns["UnitPrice"] != null)
                    gridOrderItems.Columns["UnitPrice"].HeaderText = "Egységár";
                if (gridOrderItems.Columns["Discount"] != null)
                    gridOrderItems.Columns["Discount"].HeaderText = "Kedvezmény";
                if (gridOrderItems.Columns["Total"] != null)
                    gridOrderItems.Columns["Total"].HeaderText = "Összesen";

                lblOrderTotal.Text = $"Végösszeg: {orderTotal:F2} EUR";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a rendelési tételek betöltésekor: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        // --- ÚJ METÓDUSOK A "REPORTS" FÜLHÖZ ---

        /// <summary>
        /// Legenerálja a riportot és megjeleníti a txtReportPreview szövegdobozban.
        /// </summary>
        private async Task GenerateReportAsync()
        {
            lblStatus.Text = "Riport generálása...";
            try
            {
                // Egy StringBuilder segítségével építjük fel a szöveges riportot
                var report = new StringBuilder();
                report.AppendLine("BOOKVERSE REPORT (2025)");
                report.AppendLine("=======================");

                using (var conn = new SqliteConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // 1. Teljes eladás (Total sales)
                    // A README szerint: "sum of completed orders"
                    // Mi a "COMPLETED" VAGY "PAID" státuszú rendeléseket összegezzük
                    string salesSql = @"
                        SELECT IFNULL(SUM(oi.Qty * oi.UnitPrice - IFNULL(oi.Discount, 0)), 0)
                        FROM OrderItems oi
                        JOIN Orders o ON oi.OrderId = o.Id
                        WHERE o.Status = 'COMPLETED' OR o.Status = 'PAID'";

                    var salesCmd = new SqliteCommand(salesSql, conn);
                    // ExecuteScalar egyetlen értéket ad vissza
                    decimal totalSales = Convert.ToDecimal(await salesCmd.ExecuteScalarAsync());
                    report.AppendLine($"Total sales: {totalSales:F2} EUR");

                    // 2. Alacsony készlet (Low stock < 5)
                    report.AppendLine();
                    report.AppendLine("Books below stock threshold (5):");

                    string lowStockSql = "SELECT Title, Stock FROM Books WHERE Stock < 5 ORDER BY Stock";
                    var lowStockCmd = new SqliteCommand(lowStockSql, conn);
                    using (var reader = await lowStockCmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            report.AppendLine("- Nincs ilyen könyv.");
                        }
                        else
                        {
                            while (await reader.ReadAsync())
                            {
                                report.AppendLine($"- {reader["Title"]} ({reader["Stock"]} left)");
                            }
                        }
                    }

                    // 3. Bestseller (Best-selling book)
                    report.AppendLine();
                    report.AppendLine("Best-selling:");

                    string bestsellerSql = @"
                        SELECT b.Title, SUM(oi.Qty) AS TotalSold
                        FROM OrderItems oi
                        JOIN Books b ON oi.BookIsbn = b.Isbn
                        GROUP BY b.Title
                        ORDER BY TotalSold DESC
                        LIMIT 1";

                    var bestsellerCmd = new SqliteCommand(bestsellerSql, conn);
                    using (var reader = await bestsellerCmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            report.AppendLine("- Még nem volt eladás.");
                        }
                        else
                        {
                            await reader.ReadAsync();
                            report.AppendLine($"- {reader["Title"]} ({reader["TotalSold"]} units sold)");
                        }
                    }
                }

                // A kész riport megjelenítése a TextBox-ban
                txtReportPreview.Text = report.ToString();
                lblStatus.Text = "Riport legenerálva.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a riport generálásakor: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Hiba a riport generálásakor.";
            }
        }

        /// <summary>
        /// A "Riport Exportálása" gomb eseménykezelője.
        /// </summary>
        private async void btnExportReport_Click(object sender, EventArgs e)
        {
            try
            {
                // A TextBox tartalmát egyszerűen kiírjuk a fájlba
                File.WriteAllText(ReportFile, txtReportPreview.Text);

                // Megnyitjuk a fájlt a Jegyzettömbbel
                System.Diagnostics.Process.Start(ReportFile);
                lblStatus.Text = $"Riport sikeresen exportálva: {ReportFile}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a riport exportálásakor: {ex.Message}", "Fájl Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Hiba a riport exportálásakor.";
            }
        }

        
    } // --- Osztály vége ---
} // --- Névtér vége ---