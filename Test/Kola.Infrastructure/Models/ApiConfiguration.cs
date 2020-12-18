using Newtonsoft.Json;

namespace Kola.Infrastructure.Models
{
    public class ApiConfiguration
    {
        [JsonProperty("AuthBaseApiUrl")]
        public string AuthBaseApiUrl { get; set; }

        [JsonProperty("CatalogBaseApiUrl")]
        public string CatalogBaseApiUrl { get; set; }

        [JsonProperty("FetaureBaseApiUrl")]
        public string FetaureBaseApiUrl { get; set; }

        [JsonProperty("TokenEndPoint")]
        public string TokenEndPoint { get; set; }

        [JsonProperty("AuthSystemId")]
        public string AuthSystemId { get; set; }

        [JsonProperty("AuthSecret")]
        public string AuthSecret { get; set; }

        [JsonProperty("SystemId")]
        public string SystemId { get; set; }

        [JsonProperty("CountryPrefix")]
        public string CountryPrefix { get; set; }

        [JsonProperty("PlateformBaseApiUrl")]
        public string PlateformBaseApiUrl { get; set; }

        [JsonProperty("ApplicationBaseApiUrl")]
        public string ApplicationBaseApiUrl { get; set; }

        [JsonProperty("NotificationQueue")]
        public string NotificationQueue { get; set; }
    }
}
