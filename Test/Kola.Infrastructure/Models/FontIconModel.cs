using Xamarin.Forms;

namespace Kola.Infrastructure.Models
{
    public sealed class FontIconModel
    {
        public string Name { get; private set; }
        public Style Style { get; private set; }

        public FontIconModel(string pname)
        {
            Name = pname;
        }
        public FontIconModel WithFont(string pfont)
        {
            // Convert Unicode to Char
            var font = char.ConvertFromUtf32(int.Parse(pfont, System.Globalization.NumberStyles.HexNumber));

            if (Name.Contains("-btn-"))
            {
                Style = new Style(typeof(Button))
                {
                    BaseResourceKey = Name,
                    Setters = {
                    new Setter { Property = Button.TextProperty, Value = font }
                },
                    // You can find a base style "IconBtnFont" to App.xaml
                    BasedOn = Application.Current.Resources[$"IconBtnFont"] as Style
                };
            }
            else
            {
                Style = new Style(typeof(Label))
                {
                    BaseResourceKey = Name,
                    Setters = {
                    new Setter { Property = Label.TextProperty, Value = font }
                },
                    // You can find a base style "IconFont" to App.xaml
                    BasedOn = Application.Current.Resources[$"IconFont"] as Style
                };
            }

            return this;
        }
    }
}
