using Kola.Infrastructure.Helper;
using System;
using Xamarin.Forms;

namespace Kola.Infrastructure.Converter
{
    public class MinuteToDurationConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var minutes = int.Parse((string)value);
            TimeSpan time = TimeSpan.FromMinutes(minutes);
            if (time.Days > 0)
            {
                string symbol = TranslateManagerHelper.Convert("duration_day_symbol");
                return $"{time.Days}{symbol}{time.Hours}h{time.Minutes}min";
            }
            else if (time.Hours > 0)
            {
                return $"{time.Hours}h{time.Minutes}min";
            }
            else
            {
                return $"{time.Minutes}min";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}