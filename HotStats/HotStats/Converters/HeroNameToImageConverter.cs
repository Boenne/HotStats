using System;
using System.Globalization;
using System.Windows.Data;

namespace HotStats.Converters
{
    public class HeroNameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "pack://application:,,,/Resources/Portraits/" + value + "_portrait.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}