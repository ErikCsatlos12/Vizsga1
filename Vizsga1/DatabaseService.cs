using Microsoft.Data.Sqlite; // <-- ÚJ CSOMAG
using System;
using System.IO;
using System.Threading.Tasks;
using UMFST.MIP.Bookstore.Models;

namespace UMFST.MIP.Bookstore
{
    // Ez az osztály felel az adatbázis-műveletekért (EF6 helyett ADO.NET)
    public static class DatabaseService
    {
        private static string dbFile = "bookstore.sqlite";
        private static string connectionString = $"Data Source={dbFile}";

        // Törli és újra létrehozza az összes táblát
        public static async Task ResetDatabaseAsync()
        {
            // Töröljük a régi fájlt, ha létezik
            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }

            using (var conn = new SqliteConnection(connectionString))
            {
                await conn.OpenAsync();

                // --- Táblák létrehozása ---
                // Mivel nincs EF6, nekünk kell manuálisan megírni a CREATE TABLE parancsokat
                // a modelljeink alapján.

                string createAuthors = @"
                    CREATE TABLE Authors (
                        Id      TEXT PRIMARY KEY,
                        Name    TEXT,
                        Country TEXT
                    );";

                string createBooks = @"
                    CREATE TABLE Books (
                        Isbn        TEXT PRIMARY KEY,
                        Title       TEXT,
                        Price       DECIMAL,
                        Stock       INTEGER,
                        AuthorId    TEXT,
                        FOREIGN KEY(AuthorId) REFERENCES Authors(Id)
                    );";

                string createCustomers = @"
                    CREATE TABLE Customers (
                        Id      TEXT PRIMARY KEY,
                        Name    TEXT,
                        Email   TEXT
                    );";

                string createOrders = @"
                    CREATE TABLE Orders (
                        Id          TEXT PRIMARY KEY,
                        DateString  TEXT,
                        Date        DATETIME,
                        Status      TEXT,
                        CustomerId  TEXT,
                        FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
                    );";

                string createOrderItems = @"
                    CREATE TABLE OrderItems (
                        Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                        Qty         INTEGER,
                        UnitPrice   DECIMAL,
                        Discount    DECIMAL,
                        OrderId     TEXT,
                        BookIsbn    TEXT,
                        FOREIGN KEY(OrderId) REFERENCES Orders(Id),
                        FOREIGN KEY(BookIsbn) REFERENCES Books(Isbn)
                    );";

                string createPayments = @"
                    CREATE TABLE Payments (
                        Id          TEXT PRIMARY KEY,
                        OrderId     TEXT,
                        Method      TEXT,
                        Amount      DECIMAL,
                        Captured    INTEGER
                    );";

                // Parancsok futtatása
                await (new SqliteCommand(createAuthors, conn)).ExecuteNonQueryAsync();
                await (new SqliteCommand(createBooks, conn)).ExecuteNonQueryAsync();
                await (new SqliteCommand(createCustomers, conn)).ExecuteNonQueryAsync();
                await (new SqliteCommand(createOrders, conn)).ExecuteNonQueryAsync();
                await (new SqliteCommand(createOrderItems, conn)).ExecuteNonQueryAsync();
                await (new SqliteCommand(createPayments, conn)).ExecuteNonQueryAsync();
            }
        }

        // Egy segítő metódus, ami biztonságosan ad paramétereket a parancshoz
        public static void AddParameter(SqliteCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }
    }
}