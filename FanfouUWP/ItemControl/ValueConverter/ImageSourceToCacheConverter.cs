using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using FanfouUWP.Utils;

namespace FanfouUWP.ItemControl.ValueConverter
{
    public class ImageSourceToCacheConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var path = value as string;
            if (String.IsNullOrEmpty(path)) return null;
            var imageFileUri = new Uri(path);
            if (imageFileUri.Scheme == "http" || imageFileUri.Scheme == "https")
            {
                return WebDataCache.GetLocalUriAsync(imageFileUri).Result;
            }

            // 不是网络图片,应用内的素材  
            var bm = new BitmapImage(imageFileUri);
            return bm;
        }
    }
}