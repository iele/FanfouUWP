using System;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ValueConverter
{
    public sealed class RetweetToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value as string == "")
                return "";
            return "转自" + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}