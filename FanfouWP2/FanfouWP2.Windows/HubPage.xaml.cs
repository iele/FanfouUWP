using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Utils;
using Windows.UI.Xaml.Controls.Primitives;

namespace FanfouWP2
{
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private User currentUser = new User();
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private ObservableCollection<Status> mentions = new ObservableCollection<Status>();
        private ObservableCollection<Status> publics = new ObservableCollection<Status>();

        private Status currentSelection;
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            FanfouAPI.FanfouAPI.Instance.HomeTimelineSuccess += Instance_HomeTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.HomeTimelineFailed += Instance_HomeTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.MentionTimelineSuccess += Instance_MentionTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.MentionTimelineFailed += Instance_MentionTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.PublicTimelineSuccess += Instance_PublicTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.PublicTimelineFailed += Instance_PublicTimelineFailed;

            this.send.StatusUpdateSuccess += send_StatusUpdateSuccess;
            this.send.StatusUpdateFailed += send_StatusUpdateFailed;
        }

        void send_StatusUpdateFailed(object sender, FailedEventArgs e)
        {
        }

        void send_StatusUpdateSuccess(object sender, EventArgs e)
        {
            this.sendPopup.IsOpen = false;
            loading.Visibility = Visibility.Visible;
            if (mainHubSection.ContentTemplate == this.StatusDataTemplate)
            {
                FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60);
            }
            if (mainHubSection.ContentTemplate == this.MentionDataTemplate)
            {
                FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60);
            }
            if (mainHubSection.ContentTemplate == this.PublicDataTemplate)
            {
                FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60);
            }
        }

        void Instance_PublicTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_PublicTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            this.publics.Clear();
            foreach (var item in ss)
                this.publics.Add(item);
        }

        private void Instance_MentionTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_MentionTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            this.mentions.Clear();
            foreach (var item in ss)
                this.mentions.Add(item);
        }

        private void Instance_HomeTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_HomeTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            this.statuses.Clear();
            foreach (var item in ss)
                this.statuses.Add(item);
        }


        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel["statuses"] = statuses;
            this.defaultViewModel["mentions"] = mentions;
            this.defaultViewModel["publics"] = publics;

            currentUser = FanfouAPI.FanfouAPI.Instance.currentUser;
            this.defaultViewModel["currentUser"] = currentUser;

            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60);
            this.defaultViewModel["hubHeader"] = "我的消息";

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
            var item = e.ClickedItem as Status;
            Frame.Navigate(typeof(StatusPage), item);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            this.sendPopup.IsOpen = true;
        }
        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60);
            this.defaultViewModel["hubHeader"] = "我的消息";
            mainHubSection.ContentTemplate = StatusDataTemplate;
        }

        private void MentionButton_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60);
            this.defaultViewModel["hubHeader"] = "提及我的";
            mainHubSection.ContentTemplate = MentionDataTemplate;
        }
        private void PublicButton_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60);
            this.defaultViewModel["hubHeader"] = "随便看看";
            mainHubSection.ContentTemplate = PublicDataTemplate;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

            loading.Visibility = Visibility.Visible;
            if (mainHubSection.ContentTemplate == this.StatusDataTemplate)
            {
                FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60);
            }
            if (mainHubSection.ContentTemplate == this.MentionDataTemplate)
            {
                FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60);
            }
            if (mainHubSection.ContentTemplate == this.PublicDataTemplate)
            {
                FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60);
            }
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainHubSection.ContentTemplate == this.StatusDataTemplate)
            {
                Frame.Navigate(typeof(TimelinePage), FanfouWP2.TimelinePage.PageType.Statuses);
            }
            if (mainHubSection.ContentTemplate == this.MentionDataTemplate)
            {
                Frame.Navigate(typeof(TimelinePage), FanfouWP2.TimelinePage.PageType.Mentions);
            }
            if (mainHubSection.ContentTemplate == this.PublicDataTemplate)
            {
                Frame.Navigate(typeof(TimelinePage), FanfouWP2.TimelinePage.PageType.Publics);
            }
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void statusesGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSelection = (sender as GridView).SelectedItem as Status;
            if ((sender as GridView).SelectedIndex != -1)
            {
                commandBar.Visibility = Visibility.Visible;
                commandBar.IsOpen = true;
            }
            else
            {
                commandBar.IsOpen = false;
                commandBar.Visibility = Visibility.Collapsed;
            }
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.sendPopup.IsOpen = true;
            this.send.ChangeMode(CustomControl.SendSettingsFlyout.SendMode.Reply, currentSelection);
        }

        private void RepostButton_Click(object sender, RoutedEventArgs e)
        {
            this.sendPopup.IsOpen = true;
            this.send.ChangeMode(CustomControl.SendSettingsFlyout.SendMode.Repose, currentSelection);
        }

        private void FavButton1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void send_BackClick(object sender, BackClickEventArgs e)
        {
            this.sendPopup.IsOpen = false;
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), currentSelection.user);
        }

    }
}
