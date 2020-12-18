using Kola.Infrastructure.Helper;
using System;
using Xamarin.Forms;

namespace Kola.Infrastructure.Converter
{
    public class PassengersConvert : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || (int)value == 0)
                return string.Empty;
            
            var nbPassenger = (int)value;

            if (nbPassenger < 2)
            {
                string symbol = TranslateManagerHelper.Convert("passenger").ToUpper();
                return $"{nbPassenger} {symbol}";
            }
            else
            {
                string symbol = TranslateManagerHelper.Convert("passengers").ToUpper();                
                return $"{nbPassenger} {symbol}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}