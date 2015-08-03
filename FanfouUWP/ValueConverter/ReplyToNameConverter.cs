using System;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ValueConverter
{
    public sealed class ReplyToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value as string == "")
                return "";
            return "回复" + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}