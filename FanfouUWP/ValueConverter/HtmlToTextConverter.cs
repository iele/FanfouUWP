using System;
using System.Net;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ValueConverter
{
    public sealed class HtmlToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || (value as string).Equals(""))
                return "";
            return WebUtility.HtmlDecode(value as string).Replace("<strong>", "").Replace("</strong>", "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}