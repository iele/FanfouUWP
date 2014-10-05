using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml.Data;

namespace FanfouWP2.ItemControl.ValueConverter
{
    public sealed class StatusesToTimeStringConverter : IValueConverter
    {
        private DateToTextConverter dttc = new DateToTextConverter();
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var a = ((ObservableCollection<Status>)value)[0];
                var b = ((ObservableCollection<Status>)value)[((ObservableCollection<Status>)value).Count - 1];
                return dttc.Convert(a.created_at, null, null, null) + " ~ " + dttc.Convert(b.created_at, null, null, null);
            }
            catch (Exception) {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
