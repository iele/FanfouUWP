using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FanfouUWP.FanfouAPI.Items;
using FanfouUWP.Utils;

namespace FanfouUWP.TemplateSelector
{
    public class TimelineTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StatusTemplate { get; set; }

        public DataTemplate StatusItemControlWithImage { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if ((item as Status).photo != null && SettingStorage.Instance.showPhoto)
                return StatusItemControlWithImage;
            return StatusTemplate;
        }
    }
}