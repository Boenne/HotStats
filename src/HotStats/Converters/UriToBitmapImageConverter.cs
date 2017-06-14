using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace HotStats.Converters
{
    public class UriToBitmapImageConverter : IValueConverter
    {
        private static FileStream bitmapStreamSource;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetBitmapImage(value?.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static BitmapImage GetBitmapImage(string url)
        {
            if(string.IsNullOrEmpty(url))return new BitmapImage();
            if (bitmapStreamSource != null)
            {
                bitmapStreamSource.Close();
                bitmapStreamSource.Dispose();
                bitmapStreamSource = null;
                GC.Collect();
            }

            var bitmap = new BitmapImage();
            bitmapStreamSource = File.OpenRead(url);
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            bitmap.StreamSource = bitmapStreamSource;
            bitmap.EndInit();
            return bitmap;
        }
    }
}