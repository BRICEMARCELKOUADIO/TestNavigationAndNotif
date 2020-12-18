using System;
using System.Globalization;
using Xamarin.Forms;

namespace Kola.Infrastructure.Converter
{
    public class StringToDateConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var str = (string)value;
            var sub_str = str.Split(' ');
            try
            {
                DateTime datetime = DateTime.ParseExact(sub_str[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return datetime.ToString("dd MMM");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur survenue sur conversion Method StringToDateConverter: {value}");
                return ":";
            }           
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}