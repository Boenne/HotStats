using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using HotStats.Properties;

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
            var useMasterPortraits = Settings.Default.UseMasterPortraits;
            var path = $"{Environment.CurrentDirectory}/images/{(useMasterPortraits ? "master" : "normal")}/{hero}.png";
            var exists = File.Exists(path);
            return !exists ? "pack://application:,,,/Resources/Portraits/Unknown_portrait.png" : path;
        }
    }
}