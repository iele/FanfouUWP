using System;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ItemControl.ValueConverter
{
    public sealed class WidthToStackCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}