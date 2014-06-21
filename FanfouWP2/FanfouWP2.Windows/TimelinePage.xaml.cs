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
    public sealed partial class TimelinePage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<ObservableCollection<Status>> statuses = new ObservableCollection<ObservableCollection<Status>>();

        public enum PageType { Statuses, Mentions, Publics, Favorite };
        private PageType currentType;
        private Status currentSelection;

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public TimelinePage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.HomeTimelineSuccess += Instance_TimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.HomeTimelineFailed += Instance_TimelineFailed;

            FanfouAPI.FanfouAPI.Instance.MentionTimelineSuccess += Instance_TimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.MentionTimelineFailed += Instance_TimelineFailed;

            FanfouAPI.FanfouAPI.Instance.PublicTimelineSuccess += Instance_TimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.PublicTimelineFailed += Instance_TimelineFailed;

            FanfouAPI.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouAPI.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;

            this.send.StatusUpdateSuccess += send_StatusUpdateSuccess;
            this.send.StatusUpdateFailed += send_StatusUpdateFailed;
        }

        void Instance_FavoritesFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            statuses.Add(new ObservableCollection<Status>(ss));
        }

        void send_StatusUpdateFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void send_StatusUpdateSuccess(object sender, EventArgs e)
        {
            this.sendPopup.IsOpen = false;
            loading.Visibility = Visibility.Visible;
        }
        void Instance_TimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_TimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            statuses.Add(new ObservableCollection<Status>(ss));
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel["statuses"] = statuses;

            currentType = (PageType)e.NavigationParameter;

            loading.Visibility = Visibility.Visible;

            switch ((PageType)e.NavigationParameter)
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
                case PageType.Favorite:
                    FanfouAPI.FanfouAPI.Instance.FavoritesId(FanfouAPI.FanfouAPI.Instance.currentUser.id, 60, 1);
                    FanfouAPI.FanfouAPI.Instance.FavoritesId(FanfouAPI.FanfouAPI.Instance.currentUser.id, 60, 2);
                    this.defaultViewModel["title"] = "我的收藏";
                    break;
                default:
                    break;
            }
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
            var item = e.ClickedItem as Status;
            Frame.Navigate(typeof(StatusPage), item);
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.flipView.SelectedIndex == this.flipView.Items.Count() - 1 && this.flipView.SelectedIndex >= 1)
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
                    case PageType.Favorite:
                        FanfouAPI.FanfouAPI.Instance.FavoritesId(FanfouAPI.FanfouAPI.Instance.currentUser.id, 60, this.flipView.Items.Count() + 1);
                        break;
                    case PageType.Publics:
                        break;
                    default:
                        break;
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.statuses.Clear();
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
                    FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60, 1);
                    this.defaultViewModel["title"] = "随便看看";
                    break;
                case PageType.Favorite:
                      FanfouAPI.FanfouAPI.Instance.FavoritesId(FanfouAPI.FanfouAPI.Instance.currentUser.id, 60, 1);
                      FanfouAPI.FanfouAPI.Instance.FavoritesId(FanfouAPI.FanfouAPI.Instance.currentUser.id, 60, 2);
                       this.defaultViewModel["title"] = "我的收藏";
                    break;
                default:
                    break;
            }
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

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FavButton1_Click(object sender, RoutedEventArgs e)
        {

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
