using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMFST.MIP.Bookstore.Models
{
    // JSON segédosztályok (csak beolvasáshoz)
    public class Dimensions
    {
        [JsonProperty("w")]
        public double W { get; set; }
        [JsonProperty("h")]
        public double H { get; set; }
        [JsonProperty("d")]
        public double D { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    public class Weight
    {
        [JsonProperty("value")]
        public double Value { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    // A fő adatbázis entitás
    public class Book
    {
        [Key]
        [JsonProperty("isbn")]
        public string Isbn { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("stock")]
        public int Stock { get; set; }

        // --- Kapcsolat az Author-ral ---
        [JsonProperty("authorId")]
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }

        // --- JSON-only mezők (nem mentjük adatbázisba) ---

        [JsonProperty("categories")]
        [NotMapped] // FONTOS: EF ne mentse ezt a listát
        public List<string> CategoriesList { get; set; }

        [JsonProperty("dimensions")]
        [NotMapped]
        public Dimensions Dimensions { get; set; }

        [JsonProperty("weight")]
        [NotMapped]
        public Weight Weight { get; set; }

        // Navigációs tulajdonság (EF)
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}