using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace UMFST.MIP.Bookstore.Models
{
    public class Payment
    {
        [Key]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("captured")]
        public bool Captured { get; set; }
    }
}