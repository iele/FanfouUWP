using System;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ValueConverter
{
    public sealed class IntToMsgCountStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";
            return value + "个对话";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}