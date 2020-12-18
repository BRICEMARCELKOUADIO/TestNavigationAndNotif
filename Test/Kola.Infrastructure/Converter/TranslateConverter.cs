using Kola.Infrastructure.Helper;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Kola.Infrastructure.Converter
{
    public class TranslateConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null && string.IsNullOrEmpty(value.ToString()))
                return "";

            var text = value as string;

            return TranslateManagerHelper.Convert(text);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
