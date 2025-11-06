using Newtonsoft.Json;
using System.Collections.Generic;

namespace UMFST.MIP.Bookstore.Models
{
    // Ez az osztály fogja össze a teljes JSON fájlt
    public class RootData
    {
        [JsonProperty("store")]
        public Store Store { get; set; }

        [JsonProperty("authors")]
        public List<Author> Authors { get; set; }

        [JsonProperty("books")]
        public List<Book> Books { get; set; }

        [JsonProperty("orders")]
        public List<Order> Orders { get; set; }

        [JsonProperty("payments")]
        public List<Payment> Payments { get; set; }
    }
}