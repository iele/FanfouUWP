using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FanfouWP2.FanfouAPI;

namespace FanfouWP2.ItemControl.TemplateSelector
{
    public class TimelineTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StatusTemplate { get; set; }

        public DataTemplate UserTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is User)
                return UserTemplate;
            return StatusTemplate;
        }
    }
}