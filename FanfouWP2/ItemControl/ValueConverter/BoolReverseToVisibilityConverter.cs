using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FanfouWP2.ItemControl.ValueConverter
{
    public sealed class BoolReverseToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}