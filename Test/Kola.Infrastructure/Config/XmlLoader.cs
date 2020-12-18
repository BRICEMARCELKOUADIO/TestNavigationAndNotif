using Kola.Infrastructure.Config.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms.Internals;

namespace Kola.Infrastructure.Config
{
    public class XmlLoader : IXmlLoader
    {
        #region Implementation Methods
        public async Task<Dictionary<string, string>> GetAsync(string pfolder, string pfileName, string pnodeName, string pattributeValueName = "value")
        {
            var result = new Dictionary<string, string>();

            var file = $"{pfileName}.xml";
            var type = this.GetType();
            var resource = $"{type.Namespace}.{pfolder}.{file}";
            using (var stream = type.Assembly.GetManifestResourceStream(resource))
            {
                try
                {
                    using (var reader = new StreamReader(stream))
                    {
                        try
                        {
                            var xDoc = XDocument.Parse(await reader.ReadToEndAsync());

                            if (xDoc.Root != null)
                            {
                                var xElements = xDoc.Root.Elements($"{pnodeName}");
                                xElements.Where(x => !result.ContainsKey(x.Attribute("name")?.Value))
                                    .ForEach(y => result.Add(y.Attribute("name")?.Value, y.Attribute($"{pattributeValueName}")?.Value));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"XmlLoader: Error StreamReader(stream) ==> {ex.InnerException}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"XmlLoader: Error Type.Assembly.GetManifestResourceStream(resource) ==> {ex.InnerException}");
                }
            }
            return result;
        }
        #endregion
    }
}
