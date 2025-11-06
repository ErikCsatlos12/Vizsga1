using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UMFST.MIP.Bookstore.Models // Figyelj, hogy a névtér .Models-ra végződjön!
{
    public class Author
    {
        [Key] // Adatbázis elsődleges kulcs
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        // Navigációs tulajdonság (EF)
        public virtual ICollection<Book> Books { get; set; }
    }
}