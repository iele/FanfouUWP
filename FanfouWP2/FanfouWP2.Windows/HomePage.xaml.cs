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
        private ObservableCollection<ObservableCollection<Status>> statuses = new ObservableCollection<ObservableCollection<Status>>();

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
            statuses.Clear();
            statuses.Add(new ObservableCollection<Status>(ss));
        }

        private void Instance_MentionTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_MentionTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            if (ss.Count != 0)
                statuses.Add(new ObservableCollection<Status>(ss));
        }

        private void Instance_HomeTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_HomeTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            if (ss.Count != 0)
                statuses.Add(new ObservableCollection<Status>(ss));
        }


        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel["statuses"] = statuses;

            currentUser = FanfouAPI.FanfouAPI.Instance.currentUser;
            this.defaultViewModel["currentUser"] = currentUser;

            loading.Visibility = Visibility.Visible;
            switch (currentType)
            {
                case PageType.Statuses:
                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 2);
                    this.defaultViewModel["title"] = "我的消息";
                    break;
                case PageType.Mentions:
                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 2);
                    this.defaultViewModel["title"] = "提及我的";
                    break;
                case PageType.Publics:
                    this.LeftButton.Visibility = Visibility.Collapsed;
                    this.RightButton.Visibility = Visibility.Collapsed;
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
            this.statuses.Clear();
            loading.Visibility = Visibility.Visible;

            switch (currentType)
            {
                case PageType.Statuses:
                    this.LeftButton.Visibility = Visibility.Visible;
                    this.RightButton.Visibility = Visibility.Visible;
                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
                    this.defaultViewModel["title"] = "我的消息";
                    break;
                case PageType.Mentions:
                    this.LeftButton.Visibility = Visibility.Visible;
                    this.RightButton.Visibility = Visibility.Visible;
                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
                    this.defaultViewModel["title"] = "提及我的";
                    break;
                case PageType.Publics:
                    this.LeftButton.Visibility = Visibility.Collapsed;
                    this.RightButton.Visibility = Visibility.Collapsed;
                    FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60, 1);
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
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
        private void StatusesMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Statuses;

            this.LeftButton.Visibility = Visibility.Visible;
            this.RightButton.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
            this.defaultViewModel["title"] = "我的消息";
        }

        private void MentionsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Mentions;

            this.LeftButton.Visibility = Visibility.Visible;
            this.RightButton.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
            this.defaultViewModel["title"] = "提及我的";
        }

        private void PublicsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Publics;

            this.LeftButton.Visibility = Visibility.Collapsed;
            this.RightButton.Visibility = Visibility.Collapsed;
            FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60);
            this.defaultViewModel["title"] = "随便看看";
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
        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.defaultViewModel["page"] = "第" + (this.flipView.SelectedIndex + 1).ToString() + "页";

            if (this.flipView.SelectedIndex == this.flipView.Items.Count() - 1)
            {
                loading.Visibility = Visibility.Visible;
                switch (currentType)
                {
                    case PageType.Statuses:
                        FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, this.flipView.Items.Count() + 1);
                        break;
                    case PageType.Mentions:
                        FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, this.flipView.Items.Count() + 1);
                        break;
                    case PageType.Publics:
                        break;
                    default:
                        break;
                }
            }
        }

        private void flipView_SizeChanged(object sender, SizeChangedEventArgs e)
        {

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
    }
}
