using Kola.Infrastructure.Helper;
using System;
using Xamarin.Forms;

namespace Kola.Infrastructure.Converter
{
    public class EscaleConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var nbEscale = (int)value;
            if (nbEscale == 0)
            {
                string symbol = TranslateManagerHelper.Convert("direct");
                return symbol;
            }
            else if (nbEscale == 1)
            {
                string symbol = TranslateManagerHelper.Convert("escale");
                return $"{nbEscale} {symbol}";
            }
            else
            {
                string symbol = TranslateManagerHelper.Convert("escales");
                return $"{nbEscale} {symbol}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion        
    }
}