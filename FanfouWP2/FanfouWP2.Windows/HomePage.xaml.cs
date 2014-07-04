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

namespace FanfouWP2
{
    public sealed partial class HomePage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private User currentUser = new User();
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private ObservableCollection<Status> mentions = new ObservableCollection<Status>();
        private ObservableCollection<Status> publics = new ObservableCollection<Status>();

        public enum PageType { Statuses, Mentions, Publics };
        private PageType currentType = PageType.Statuses;

        private Status currentClick;

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public HomePage()
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

            this.status.UserButtonClick += status_UserButtonClick;
            this.status.ReplyButtonClick += status_ReplyButtonClick;
            this.status.RepostButtonClick += status_RepostButtonClick;
            this.status.FavButtonClick += status_FavButtonClick;
        }

        private void status_FavButtonClick(object sender, RoutedEventArgs e)
        {
        }

        void status_RepostButtonClick(object sender, RoutedEventArgs e)
        {
            this.sendPopup.IsOpen = true;
            this.send.ChangeMode(CustomControl.SendSettingsFlyout.SendMode.Repose, currentClick);
        }

        void status_ReplyButtonClick(object sender, RoutedEventArgs e)
        {
            this.sendPopup.IsOpen = true;
            this.send.ChangeMode(CustomControl.SendSettingsFlyout.SendMode.Reply, currentClick);
        }

        void status_UserButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), currentClick.user);
        }

        void send_StatusUpdateFailed(object sender, FailedEventArgs e)
        {
        }

        void send_StatusUpdateSuccess(object sender, EventArgs e)
        {
            this.sendPopup.IsOpen = false;
            loading.Visibility = Visibility.Visible;
        }

        void Instance_PublicTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_PublicTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            Utils.StatusesReform.reform(this.publics, ss);
        }

        private void Instance_MentionTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_MentionTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            Utils.StatusesReform.reform(this.mentions, ss);
        }

        private void Instance_HomeTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_HomeTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            Utils.StatusesReform.reform(this.statuses, ss);
        }


        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel["statuses"] = statuses;
            this.defaultViewModel["mentions"] = mentions;
            this.defaultViewModel["publics"] = publics;

            currentUser = FanfouAPI.FanfouAPI.Instance.currentUser;
            this.defaultViewModel["currentUser"] = currentUser;

            loading.Visibility = Visibility.Visible;
            switch (currentType)
            {
                case PageType.Statuses:
                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
                    this.defaultViewModel["title"] = "我的消息";
                    break;
                case PageType.Mentions:
                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
                    this.defaultViewModel["title"] = "提及我的";
                    break;
                case PageType.Publics:
                    FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60, 1);
                    this.defaultViewModel["title"] = "随便看看";
                    break;
                default:
                    break;
            }

            this.defaultViewModel["page"] = "第1页";
        }

        #region NavigationHelper 注册

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
            currentClick = e.ClickedItem as Status;
            this.status.setStatus(currentClick);
            this.statusPopup.IsOpen = true;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            this.sendPopup.IsOpen = true;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;

            switch (currentType)
            {
                case PageType.Statuses:
                    if (statuses.Count == 0)
                        FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
                    else
                    {
                        FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, since_id: statuses.First().id);
                    }
                    this.defaultViewModel["title"] = "我的消息";
                    break;
                case PageType.Mentions:
                    if (mentions.Count == 0)
                        FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
                    else
                    {
                        FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, since_id: mentions.First().id);
                    }
                    this.defaultViewModel["title"] = "提及我的";
                    break;
                case PageType.Publics:
                    if (publics.Count == 0)
                        FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60, 1);
                    else
                    {
                        FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60, since_id: publics.First().id);
                    }
                    this.defaultViewModel["title"] = "随便看看";
                    break;
                default:
                    break;
            }
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void send_BackClick(object sender, BackClickEventArgs e)
        {
            this.sendPopup.IsOpen = false;
        }

        private void FavAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TimelinePage),
                new KeyValuePair<TimelinePage.PageType, object>(TimelinePage.PageType.Favorite,
                    FanfouAPI.FanfouAPI.Instance.currentUser));
        }

        private void pageTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(this.pageTitle as FrameworkElement);
        }
        private void StatusesMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Statuses;

            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
            this.defaultViewModel["title"] = "我的消息";
            this.statusesGridView.ItemsSource = this.defaultViewModel["statuses"];
        }

        private void MentionsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Mentions;

            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
            this.defaultViewModel["title"] = "提及我的";
            this.statusesGridView.ItemsSource = this.defaultViewModel["mentions"];
        }

        private void PublicsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Publics;

            FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60);
            this.defaultViewModel["title"] = "随便看看";
            this.statusesGridView.ItemsSource = this.defaultViewModel["publics"];
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));
        }

        private void DirectButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DirectMessagePage));
        }

        private void status_BackClick(object sender, BackClickEventArgs e)
        {
            this.statusPopup.IsOpen = false;
        }

        private void MenuFlyout_Closed(object sender, object e)
        {
            rotation.Rotation = 0;
        }

        private void MenuFlyout_Opened(object sender, object e)
        {
            rotation.Rotation = 180;
        }
    }
}
