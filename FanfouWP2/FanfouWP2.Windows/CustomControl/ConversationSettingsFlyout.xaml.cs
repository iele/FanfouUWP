using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“设置浮出控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=273769 上有介绍

namespace FanfouWP2.CustomControl
{
    public sealed partial class ConversationSettingsFlyout : SettingsFlyout
    {
        private User user;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<DirectMessage> list = new ObservableCollection<DirectMessage>();
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public ConversationSettingsFlyout()
        {
            this.InitializeComponent();

            FanfouAPI.FanfouAPI.Instance.DirectMessageConversationSuccess += Instance_DirectMessageConversationSuccess;
            FanfouAPI.FanfouAPI.Instance.DirectMessageConversationFailed += Instance_DirectMessageConversationFailed;
        }

        void Instance_DirectMessageConversationFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_DirectMessageConversationSuccess(object sender, EventArgs e)
        {
            var ss = sender as List<DirectMessage>;
            ss.Reverse();
            this.list = new ObservableCollection<DirectMessage>(ss);
            this.defaultViewModel["list"] = list;
        }

        public void setUser(User user){
            this.user = user;
            this.Title = "和" + user.screen_name + "的对话";
            this.list.Clear();
            FanfouAPI.FanfouAPI.Instance.DirectMessagesConversation(user.id, 60);
        }
    }
}
