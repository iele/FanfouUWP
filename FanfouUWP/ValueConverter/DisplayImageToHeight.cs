using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ValueConverter
{
    public sealed class DisplayImageToHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 0.0;
            else
                return 100.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}