using FanfouWP2.FanfouAPI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FanfouWP2.ItemControl.TemplateSelector
{

    public class MessageItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessageOtherTemplate { get; set; }

        public DataTemplate MessageSelfTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var i = item as DirectMessage;
//            if (i.sender_id == FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
//                return MessageSelfTemplate;
            return MessageOtherTemplate;
        }
    }
}