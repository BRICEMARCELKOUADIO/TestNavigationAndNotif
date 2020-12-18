using System;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kola.Infrastructure.Design
{
    [ContentProperty("BackgroundImage")]
    public class BackgroundImageResourceExtension : IMarkupExtension
    {
        public string BackgroundImage { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(this.BackgroundImage))
            {
                return null;
            }

            var imageSource = ImageSource.FromResource($"Kola.Infrastructure.Design.Images.{this.BackgroundImage}.png");

            return imageSource;
        }
    }
}
