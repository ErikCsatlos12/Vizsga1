using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMFST.MIP.Bookstore.Models
{
    // JSON segédosztály (nem mentjük adatbázisba)
    public class PaymentInfo
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("transactions")]
        public List<object> Transactions { get; set; } // Itt most nem részletezzük
    }

    public class Order
    {
        [Key]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("date")]
        public string DateString { get; set; } // Stringként olvassuk be a "BAD_DATE" miatt

        [NotMapped]
        public DateTime Date { get; set; } // Ezt majd mi töltjük fel validálás után

        [JsonProperty("status")]
        public string Status { get; set; }

        // --- Kapcsolat a Customer-rel ---
        [NotMapped] // JSON-only beágyazott objektum
        [JsonProperty("customer")]
        public Customer CustomerJson { get; set; }

        public string CustomerId { get; set; } // Valódi adatbázis idegen kulcs
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        // --- JSON-only mezők ---
        [JsonProperty("items")]
        [NotMapped]
        public List<OrderItem> Items { get; set; } // Ezt az importáláskor dolgozzuk fel

        [JsonProperty("payment")]
        [NotMapped]
        public PaymentInfo PaymentJson { get; set; }
    }
}