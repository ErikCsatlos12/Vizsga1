using Newtonsoft.Json;
using System.Collections.Generic;

namespace UMFST.MIP.Bookstore.Models
{
    // Segédosztályok
    public class Address
    {
        [JsonProperty("line1")]
        public string Line1 { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
    }

    public class Employee
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("hiredAt")]
        public string HiredAt { get; set; }
    }

    // Fő osztály
    public class Store
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("address")]
        public Address Address { get; set; }
        [JsonProperty("employees")]
        public List<Employee> Employees { get; set; }
    }
}