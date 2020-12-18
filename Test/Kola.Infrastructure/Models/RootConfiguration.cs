using Newtonsoft.Json;

namespace Kola.Infrastructure.Models
{
    public class RootConfiguration
    {
        [JsonProperty("Api")]
        public ApiConfiguration ApiConfiguration { get; set; }
    }
}
