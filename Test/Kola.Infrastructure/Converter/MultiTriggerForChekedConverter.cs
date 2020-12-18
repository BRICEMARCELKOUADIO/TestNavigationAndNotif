using System;
using System.Globalization;
using Xamarin.Forms;

namespace Kola.Infrastructure.Converter
{
    public class MultiTriggerForChekedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var ok = value is bool;

            
            if (ok) // Value is checked
            {
                return true;
            }
            else
            {
                return false;   // not cheked
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }    
}
