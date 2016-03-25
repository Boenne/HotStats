using System;
using System.Globalization;
using System.Windows.Data;

namespace HotStats.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{(int) value:N0}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}