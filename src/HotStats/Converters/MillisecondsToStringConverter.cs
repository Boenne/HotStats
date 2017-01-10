using System;
using System.Globalization;
using System.Windows.Data;

namespace HotStats.Converters
{
    public class MillisecondsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var milliseconds = (long) value;
            return TimeSpan.FromMilliseconds(milliseconds).ToString("g");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}