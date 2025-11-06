using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMFST.MIP.Bookstore.Models
{
    public class OrderItem
    {
        [Key] // Nincs a JSON-ban, ezért automatikusan generáljuk
        public int Id { get; set; }

        [JsonProperty("qty")]
        public int Qty { get; set; }

        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty("discount")]
        public decimal? Discount { get; set; } // Nullable, mert nem mindig van

        // --- Kapcsolat az Order-rel ---
        public string OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        // --- Kapcsolat a Book-kal ---
        [JsonProperty("isbn")]
        public string BookIsbn { get; set; }
        [ForeignKey("BookIsbn")]
        public virtual Book Book { get; set; }
    }
}