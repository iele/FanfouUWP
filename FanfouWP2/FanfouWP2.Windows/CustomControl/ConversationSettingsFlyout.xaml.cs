using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;

//“设置浮出控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=273769 上有介绍

namespace FanfouWP2.CustomControl
{
    public sealed partial class ConversationSettingsFlyout : SettingsFlyout
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<DirectMessage> list = new ObservableCollection<DirectMessage>();
        private User user;

        public ConversationSettingsFlyout()
        {
            InitializeComponent();

            FanfouAPI.FanfouAPI.Instance.DirectMessageConversationSuccess += Instance_DirectMessageConversationSuccess;
            FanfouAPI.FanfouAPI.Instance.DirectMessageConversationFailed += Instance_DirectMessageConversationFailed;

            FanfouAPI.FanfouAPI.Instance.DirectMessageNewSuccess += Instance_DirectMessageNewSuccess;
            FanfouAPI.FanfouAPI.Instance.DirectMessageNewFailed += Instance_DirectMessageNewFailed;
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        private void Instance_DirectMessageNewFailed(object sender, FailedEventArgs e)
        {
        }

        private void Instance_DirectMessageNewSuccess(object sender, EventArgs e)
        {
            message.Text = "";
            list.Add(sender as DirectMessage);
            if (list.Count != 0)
                listView.ScrollIntoView(list.Last());
        }

        private void Instance_DirectMessageConversationFailed(object sender, FailedEventArgs e)
        {
        }

        private void Instance_DirectMessageConversationSuccess(object sender, EventArgs e)
        {
            var ss = sender as List<DirectMessage>;
            ss.Reverse();
            list = new ObservableCollection<DirectMessage>(ss);
            defaultViewModel["list"] = list;
            if (list.Count != 0)
                listView.ScrollIntoView(list.Last());
        }

        public void setUser(User user)
        {
            this.user = user;
            Title = "和" + user.screen_name + "的对话";
            list.Clear();
            FanfouAPI.FanfouAPI.Instance.DirectMessagesConversation(user.id, 60);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (message.Text.Count() > 0)
            {
                FanfouAPI.FanfouAPI.Instance.DirectMessagesNew(user.id, message.Text);
            }
        }
    }
}