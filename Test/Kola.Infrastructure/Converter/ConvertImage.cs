using System;
using System.Globalization;
using Xamarin.Forms;

namespace Kola.Infrastructure.Converter
{
    public class ConvertImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string source = value as string;
            if (string.IsNullOrEmpty(source))
            {
                Console.Write($"ConvertImage: Image Resource not found ==> {source}.png");
                return null;
            }

            var imageSource = ImageSource.FromResource($"Kola.Infrastructure.Design.Images.{source}.png");
            return imageSource;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
