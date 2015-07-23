using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.ItemControl.ValueConverter
{
    public sealed class DateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var cultureInfo = new CultureInfo("en-US");
                string format = "ddd MMM d HH:mm:ss zz00 yyyy";
                string stringValue = DateTime.Now.ToString(format, cultureInfo);
                DateTime datetime = DateTime.ParseExact(value as string, format, cultureInfo);
                DateTime currenttime = DateTime.Now;

                string dateDiff = "";

                var ts1 = new TimeSpan(currenttime.Ticks);
                var ts2 = new TimeSpan(datetime.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                if (ts1 < ts2)
                {
                    if (ts2.Subtract(ts1).Duration().TotalMinutes > 10)
                        return "未来";
                    return "刚刚";
                }
                if (ts.Days != 0)
                {
                    if (ts.Days < 60)
                        return ts.Days + "天";
                    return datetime.Year + "年" + datetime.Month + "月" + datetime.Day + "日";
                }
                if (ts.Hours != 0)
                    return ts.Hours + "小时";
                if (ts.Minutes != 0)
                    return ts.Minutes + "分钟";
                if (ts.Seconds != 0)
                    return ts.Seconds + "秒";
                return dateDiff;
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