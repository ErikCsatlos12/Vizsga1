using System.Data.Entity;
using UMFST.MIP.Bookstore.Models; // Fontos, hogy elérjük a Models mappában lévő osztályokat

// A névtérnek a projekt fő névterének kell lennie (a szabályzat szerint)
namespace UMFST.MIP.Bookstore
{
    // A NÉV KÖTELEZŐEN AppContext!
    // Az osztály a DbContext-ből öröklődik (ez az EF6 lényege)
    public class AppContext : DbContext
    {
        // Konstruktor:
        // Ez mondja meg az Entity Framework-nek, hogy a "BookstoreDbConnection" nevű
        // kapcsolatleíró stringet keresse az App.config fájlban.
        public AppContext() : base("BookstoreDbConnection")
        {
        }

        // --- Adatbázis Táblák (DbSet) ---
        // A README által kért táblák.
        // Az EF ezek alapján hozza létre az adatbázis sémát.

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}