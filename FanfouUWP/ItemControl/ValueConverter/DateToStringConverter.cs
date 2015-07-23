using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ItemControl.ValueConverter
{
    public sealed class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var cultureInfo = new CultureInfo("en-US");
                string format = "ddd MMM d HH:mm:ss zz00 yyyy";
                DateTime datetime = DateTime.ParseExact(value as string, format, cultureInfo);
                return datetime.ToString("yyyy年MMMd日 HH:mm:ss");
            }
            catch
            {
                return "未知";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}