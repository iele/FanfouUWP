using System;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ItemControl.ValueConverter
{
    public sealed class TextToReplyTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "回复给" + value + "的消息";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}