using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FanfouWP2.FanfouAPI.Items;

namespace FanfouWP2.ItemControl.TemplateSelector
{
    public class TimelineTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StatusTemplate { get; set; }

        public DataTemplate RefreshTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if ((item as Status).is_refresh == true)
                return RefreshTemplate;
            return StatusTemplate;
        }
    }
}