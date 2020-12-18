using Kola.Infrastructure.Config.Contract;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Kola.Infrastructure.Config
{
    public class JsonLoader<T> : IJsonLoader<T>
    {
        public async Task<T> GetAsync( string penvironment = "Development")
         {
            T response = default;

            var file = $"app-settings.{penvironment}.json";
            var type = this.GetType();
            var resource = $"{type.Namespace}.Files.{file}";
            using (var stream = type.Assembly.GetManifestResourceStream(resource))
            {
                try
                {
                    using (var reader = new StreamReader(stream))
                    {
                        try
                        {
                            var jsonString = await reader.ReadToEndAsync();
                            if (jsonString != null)
                            {
                                response = JsonConvert.DeserializeObject<T>(jsonString);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"JsonLoader: Error StreamReader(stream) ==> {ex.InnerException}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"JsonLoader: Environment not Found,  Error Type.Assembly.GetManifestResourceStream(resource) ==> {ex.InnerException}");
                }
            }
                
            return response;
        }
    }
}
