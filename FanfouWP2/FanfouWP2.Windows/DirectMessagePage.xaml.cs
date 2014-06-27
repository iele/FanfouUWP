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

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace FanfouWP2
{
    public sealed partial class DirectMessagePage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<ObservableCollection<DirectMessageItem>> messages = new ObservableCollection<ObservableCollection<DirectMessageItem>>();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public DirectMessagePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.DirectMessageConversationListSuccess += Instance_DirectMessageConversationListSuccess;
            FanfouAPI.FanfouAPI.Instance.DirectMessageConversationListFailed += Instance_DirectMessageConversationListFailed;
        }

        void Instance_DirectMessageConversationListFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;

        }

        void Instance_DirectMessageConversationListSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<DirectMessageItem>;
            if (ss.Count != 0)
                messages.Add(new ObservableCollection<DirectMessageItem>(ss));
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            loading.Visibility = Visibility.Visible;

            this.defaultViewModel["messages"] = messages;

            FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(1, 60);

            this.defaultViewModel["page"] = "第1页";
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper 注册

        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// 
        /// 应将页面特有的逻辑放入用于
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// 和 <see cref="GridCS.Common.NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as DirectMessageItem).dm.recipient.id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                message.setUser((e.ClickedItem as DirectMessageItem).dm.sender);
            else
                message.setUser((e.ClickedItem as DirectMessageItem).dm.recipient);
      
            popup.IsOpen = true;
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.defaultViewModel["page"] = "第" + (this.flipView.SelectedIndex + 1).ToString() + "页";

            if (this.flipView.SelectedIndex == this.flipView.Items.Count() - 1)
            {
                loading.Visibility = Visibility.Visible;
                FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(this.flipView.Items.Count() + 1, 60);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.messages.Clear();
            loading.Visibility = Visibility.Visible;

            FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(1, 60);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.flipView.SelectedIndex > 0)
                this.flipView.SelectedIndex--;
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.flipView.SelectedIndex < this.flipView.Items.Count() - 1)
                this.flipView.SelectedIndex++;
        }
        private void flipView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void message_BackClick(object sender, BackClickEventArgs e)
        {
            popup.IsOpen = false;
        }
    }
}
