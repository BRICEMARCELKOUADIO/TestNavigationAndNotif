using Kola.Infrastructure.Models;
using Kola.Infrastructure.Config.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Kola.Infrastructure.Design
{
    public sealed class Theme
    {
        private readonly string _iconFile = "app-icon";
        private readonly string _colorFile = "app-color";

        public List<FontIconModel> ListFontIcons { get; set; }
        public Theme()
        {
            ListFontIcons = new List<FontIconModel>();
        }

        public async Task Load(IXmlLoader xmlLoader)
        {
            var listFontIcon = await xmlLoader.GetAsync("Files", _iconFile, "icon");
            var listColor = await xmlLoader.GetAsync("Files", _colorFile, "color");

            try
            {
                // insert font icon style
                listFontIcon.ForEach(f => ListFontIcons.Add(new FontIconModel(f.Key).WithFont(f.Value)));
                ListFontIcons.ForEach(c => Application.Current.Resources[$"{c.Name}"] = c.Style);

                //insert list color 
                listColor.ForEach(c => Application.Current.Resources[$"{c.Key}"] = Color.FromHex(c.Value));
            }     
            catch(Exception ex)
            {
                Console.WriteLine($"Theme Load: Error occured during loading theme=>{ex.Message}");
            }
        }

    }
}
