using FanfouWP2.FanfouAPI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FanfouWP2.ItemControl.TemplateSelector
{

    public class ConversationItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ConversationOtherTemplate { get; set; }

        public DataTemplate ConversationSelfTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var i = item as DirectMessageItem;
            if (i.dm.sender_id == FanfouWP2.FanfouAPI.FanfouAPI.Instance.currentUser.id)
                return ConversationSelfTemplate;
            return ConversationOtherTemplate;
        }
    }
}