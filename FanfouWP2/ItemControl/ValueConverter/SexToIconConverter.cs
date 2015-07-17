using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace FanfouWP2.ItemControl.ValueConverter
{
    public sealed class SexToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value as string == "男")
            {
                return new BitmapImage(new Uri("/Assets/male.png", UriKind.Relative));
            }
            if (value as string == "女")
            {
                return new BitmapImage(new Uri("/Assets/female.png", UriKind.Relative));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}