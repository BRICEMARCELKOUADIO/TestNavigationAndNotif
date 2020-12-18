using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace Kola.Infrastructure.Converter
{
    public class ConvertIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string IconStyle = value as string;

            if (Application.Current.Resources.ContainsKey(IconStyle))
            {
                return Application.Current.Resources[$"{IconStyle}"] as Style;
            }
            else
            {
                return new KeyNotFoundException($" ConvertIcon : {IconStyle} not found in resources... ");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
