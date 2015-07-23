using System;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ItemControl.ValueConverter
{
    public sealed class BoolToFavoritedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";
            return (bool) value ? "已收藏" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}