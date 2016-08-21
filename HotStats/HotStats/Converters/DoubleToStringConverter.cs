using System;
using System.Globalization;
using System.Windows.Data;

namespace HotStats.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int notADouble;
            if (value == null) return string.Empty;
            return int.TryParse(value.ToString(), out notADouble)
                ? notADouble.ToString("N0")
                : $"{(double) value:N2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}