using System.Text.Json.Serialization;

namespace NewGenBridge.Helpers
{
    public class ApiData
    {
        [JsonPropertyName("countries")]
        public List<Country> Countries { get; set; }
    }

    public class Country
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("info")]
        public List<CountryInfo> Info { get; set; }
    }
    public class CountryInfo
    {
        [JsonPropertyName("group_id")]
        public int GroupId { get; set; }
        [JsonPropertyName("level_id")]
        public int LevelId { get; set; }
    }

    public class RawApiData
    {
        [JsonPropertyName("countries")]
        public List<RawCountry> Countries { get; set; }
    }

    public class RawCountry
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("info")]
        public List<RawCountryInfo> Info { get; set; }
    }
    public class RawCountryInfo
    {
        [JsonPropertyName("day")]
        public string Day { get; set; }
        [JsonPropertyName("time")]
        public string Time { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("level")]
        public string LevelCode { get; set; }

    }
}
