using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
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
            else if (value as string == "女")
            {
                return new BitmapImage(new Uri("/Assets/female.png", UriKind.Relative));
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
