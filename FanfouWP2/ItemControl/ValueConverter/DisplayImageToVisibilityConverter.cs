using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FanfouWP2.ItemControl.ValueConverter
{
    public sealed class DisplayImageToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
//if (SettingManager.GetInstance().displayImage == false)
//                   return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}