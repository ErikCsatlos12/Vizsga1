using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;             // JSON letöltéséhez
using Newtonsoft.Json;             // JSON feldolgozásához
using UMFST.MIP.Bookstore.Models;  // Az OOP modelljeink eléréséhez
using System.IO;                   // A hibalog-fájl írásához
using System.Data.Entity;          // Az adatbázis-műveletekhez
using UMFST.MIP.Bookstore.Models;
namespace UMFST.MIP.Bookstore
{
    public partial class MainWindow : Form
    {
        // --- VÁLTOZÓK ---
        // A JSON letöltési linkje a README-ből
        private const string JsonDataUrl = "https://cdn.shopify.com/s/files/1/0883/3282/8936/files/data_bookstore_final.json?v=1762418524";
        // A hibalog fájl neve (a README szerint)
        private const string ErrorLogFile = "invalid_bookstore.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        // --- ESEMÉNYKEZELŐK ---

        /// <summary>
        /// A "RESET" gomb eseménykezelője.
        /// </summary>
        private async void btnResetDatabase_Click(object sender, EventArgs e)
        {
            var valasz = MessageBox.Show("Biztosan törölni akarja a teljes adatbázist és újraimportálni az adatokat?\nMinden nem mentett változás elvész!",
                "Adatbázis Reset Megerősítése", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (valasz == DialogResult.No) return;

            // Elindítjuk a teljes Reset és Import folyamatot
            await ResetAndImportDataAsync();
        }

        /// <summary>
        /// Az "IMPORT" gomb eseménykezelője.
        /// </summary>
        private async void btnImportData_Click(object sender, EventArgs e)
        {
            var valasz = MessageBox.Show("Újra letölti és feldolgozza az adatokat a linkről?\nEz felülírja a meglévő adatokat, ha az adatbázis üres.",
                "Import Megerősítése", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (valasz == DialogResult.No) return;

            // Elindítjuk a teljes Reset és Import folyamatot
            // A README szerint az import gombnak is resetelnie kell az adatbázist
            await ResetAndImportDataAsync();
        }

        // --- FŐ LOGIKAI METÓDUSOK ---

        /// <summary>
        /// A teljes adatbázis törlését és újraimportálását vezénylő fő metódus.
        /// </summary>
        private async Task ResetAndImportDataAsync()
        {
            // Gombok letiltása a folyamat alatt
            btnImportData.Enabled = false;
            btnResetDatabase.Enabled = false;
            lblStatus.Text = "Folyamat elindítva...";

            try
            {
                // --- 1. Adatbázis törlése és újrakészítése ---
                lblStatus.Text = "Adatbázis törlése...";
                await Task.Run(() =>
                {
                    using (var context = new AppContext())
                    {
                        // Törli az adatbázist, ha létezik
                        context.Database.Delete();
                        // Újra létrehozza az adatbázist a modellek alapján
                        context.Database.Initialize(true);
                    }
                });
                lblStatus.Text = "Adatbázis sikeresen létrehozva.";

                // Hibalog törlése
                if (File.Exists(ErrorLogFile))
                    File.Delete(ErrorLogFile);

                // --- 2. JSON Letöltés ---
                lblStatus.Text = "JSON letöltése a szerverről...";
                string jsonString;
                using (HttpClient client = new HttpClient())
                {
                    // A README által megkövetelt letöltés
                    jsonString = await client.GetStringAsync(JsonDataUrl);
                }

                if (string.IsNullOrEmpty(jsonString))
                {
                    throw new Exception("A letöltött JSON üres.");
                }

                // --- 3. JSON Feldolgozás (Deserializálás) ---
                lblStatus.Text = "JSON feldolgozása...";
                // Newtonsoft.Json használata
                RootData data = JsonConvert.DeserializeObject<RootData>(jsonString);

                // --- 4. Adatok importálása és validálása ---
                lblStatus.Text = "Adatok importálása az adatbázisba...";
                using (var context = new AppContext())
                using (StreamWriter log = new StreamWriter(ErrorLogFile, append: true)) // Hibalog megnyitása
                {
                    await log.WriteLineAsync($"--- Import Napló ({DateTime.Now}) ---");

                    // Szerzők (Authors) importálása (nincs validációs szabály)
                    if (data.Authors != null)
                    {
                        context.Authors.AddRange(data.Authors);
                    }

                    // Könyvek (Books) importálása VALIDÁLÁSSAL
                    if (data.Books != null)
                    {
                        foreach (var book in data.Books)
                        {
                            // A README szerint az érvényteleneket naplózni kell
                            if (string.IsNullOrEmpty(book.Isbn) || book.Price < 0 || book.Stock < 0)
                            {
                                await log.WriteLineAsync($"[HIBA-KÖNYV] Kihagyva: ISBN='{book.Isbn}', Cím='{book.Title}', Ár={book.Price}, Készlet={book.Stock}");
                            }
                            else
                            {
                                // Csak az érvényes könyveket adjuk hozzá
                                context.Books.Add(book);
                            }
                        }
                    }

                    // Fizetések (Payments) importálása (nincs validációs szabály)
                    if (data.Payments != null)
                    {
                        context.Payments.AddRange(data.Payments);
                    }

                    // Rendelések (Orders) és kapcsolódó adatok importálása
                    if (data.Orders != null)
                    {
                        var customers = new Dictionary<string, Customer>();

                        foreach (var order in data.Orders)
                        {
                            bool validOrder = true;

                            // --- Customer validálás és hozzáadás ---
                            if (order.CustomerJson == null || string.IsNullOrEmpty(order.CustomerJson.Id) || !order.CustomerJson.Email.Contains("@"))
                            {
                                await log.WriteLineAsync($"[HIBA-CUSTOMER] Kihagyva: Rendelés ID='{order.Id}', Customer hiba.");
                                validOrder = false;
                            }
                            else
                            {
                                // Elkerüljük a duplikált ügyfelek hozzáadását
                                if (!customers.ContainsKey(order.CustomerJson.Id))
                                {
                                    customers.Add(order.CustomerJson.Id, order.CustomerJson);
                                    context.Customers.Add(order.CustomerJson);
                                }
                                order.CustomerId = order.CustomerJson.Id; // Kulcs beállítása
                            }

                            // --- Dátum validálás ---
                            if (!DateTime.TryParse(order.DateString, out DateTime parsedDate))
                            {
                                await log.WriteLineAsync($"[HIBA-DÁTUM] Kihagyva: Rendelés ID='{order.Id}', Dátum='{order.DateString}'");
                                validOrder = false;
                            }
                            else
                            {
                                order.Date = parsedDate; // Validált dátum beállítása
                            }

                            // Ha a rendelés (vagy a vevő) érvénytelen, kihagyjuk
                            if (!validOrder) continue;

                            // --- Rendelés hozzáadása ---
                            context.Orders.Add(order);

                            // --- Tételek (OrderItems) validálása ---
                            if (order.Items != null)
                            {
                                foreach (var item in order.Items)
                                {
                                    // Ellenőrizzük, hogy a könyv ISBN-je létezik-e egyáltalán
                                    // (az "UNKNOWN-ISBN" kiszűrésére)
                                    bool bookExists = context.Books.Local.Any(b => b.Isbn == item.BookIsbn);

                                    if (item.Qty <= 0 || !bookExists)
                                    {
                                        await log.WriteLineAsync($"[HIBA-TÉTEL] Kihagyva: Rendelés ID='{order.Id}', ISBN='{item.BookIsbn}', Qty={item.Qty}");
                                    }
                                    else
                                    {
                                        item.OrderId = order.Id; // Kulcs beállítása
                                        item.BookIsbn = item.BookIsbn; // Idegen kulcs beállítása
                                        context.OrderItems.Add(item);
                                    }
                                }
                            }
                        }
                    }

                    // --- 5. Mentés az adatbázisba ---
                    lblStatus.Text = "Változtatások mentése...";
                    await context.SaveChangesAsync();
                }

                lblStatus.Text = "Importálás sikeresen befejezve!";
                MessageBox.Show("Az adatbázis sikeresen törölve és újraimportálva a JSON alapján!", "Import Kész", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // TODO: Itt kell majd frissíteni a füleken lévő adatokat
                // pl: await LoadBooksAsync();
            }
            catch (Exception ex)
            {
                // Általános hiba elkapása (pl. letöltés, JSON feldolgozás)
                lblStatus.Text = "Kritikus hiba történt!";
                MessageBox.Show($"Hiba történt a folyamat során: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Gombok visszakapcsolása
                btnImportData.Enabled = true;
                btnResetDatabase.Enabled = true;
            }
        }
    }
}