using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using FanfouUWP.FanfouAPI.Items;

namespace FanfouUWP.ItemControl.ValueConverter
{
    public sealed class StatusesToTimeStringConverter : IValueConverter
    {
        private readonly DateToTextConverter dttc = new DateToTextConverter();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                Status a = ((ObservableCollection<Status>) value)[0];
                Status b = ((ObservableCollection<Status>) value)[((ObservableCollection<Status>) value).Count - 1];
                return dttc.Convert(a.created_at, null, null, null) + " ~ " +
                       dttc.Convert(b.created_at, null, null, null);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}