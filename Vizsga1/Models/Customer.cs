using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UMFST.MIP.Bookstore.Models
{
    public class Customer
    {
        [Key]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        // Navigációs tulajdonság (EF)
        public virtual ICollection<Order> Orders { get; set; }
    }
}