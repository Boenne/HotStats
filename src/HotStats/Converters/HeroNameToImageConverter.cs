using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace HotStats.Converters
{
    public class HeroNameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                value = "Unknown";
            return GetHeroImageFilePath(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetHeroImageFilePath(string hero)
        {
            const string resourcePath = "resources/portraits/{0}_portrait.png";
            const string filePath = "pack://application:,,,/Resources/Portraits/{0}_portrait.png";
            var exists = Resources.ResourceNames.Contains(string.Format(resourcePath, hero.ToLower()));
            return exists
                ? string.Format(filePath, hero)
                : string.Format(filePath, "Unknown");
        }
    }
}