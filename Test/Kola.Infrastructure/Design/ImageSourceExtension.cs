using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kola.Infrastructure.Design
{
    [ContentProperty("Source")]
    public class ImageSourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(this.Source))
            {
                return null;
            }

            var imageSource = ImageSource.FromResource($"Kola.Infrastructure.Design.Images.{this.Source}.png");

            return imageSource;
        }
    }
}
