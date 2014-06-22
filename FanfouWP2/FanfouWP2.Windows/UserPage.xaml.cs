using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“中心页”项模板在 http://go.microsoft.com/fwlink/?LinkId=321224 上有介绍

namespace FanfouWP2
{
    /// <summary>
    /// 显示分组的项集合的页。
    /// </summary>
    public sealed partial class UserPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private User user;
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private ObservableCollection<Status> favorite = new ObservableCollection<Status>();
        private ObservableCollection<User> friends = new ObservableCollection<User>();
        private ObservableCollection<User> follower = new ObservableCollection<User>();

        /// <summary>
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public UserPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            FanfouAPI.FanfouAPI.Instance.UserTimelineSuccess += Instance_UserTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.UserTimelineFailed += Instance_UserTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouAPI.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;

            FanfouAPI.FanfouAPI.Instance.UsersFollowersSuccess += Instance_UsersFollowersSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFollowersFailed += Instance_UsersFollowersFailed;

            FanfouAPI.FanfouAPI.Instance.UsersFriendsSuccess += Instance_UsersFriendsSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFriendsFailed += Instance_UsersFriendsFailed;
        }

        void Instance_UsersFriendsFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_UsersFriendsSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            this.friends.Clear();
            foreach (var item in ss)
                this.friends.Add(item);
        }

        void Instance_UsersFollowersFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_UsersFollowersSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            this.follower.Clear();
            foreach (var item in ss)
                this.follower.Add(item);
        }

        void Instance_FavoritesFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            this.favorite.Clear();
            foreach (var item in ss)
                this.favorite.Add(item);
        }

        void Instance_UserTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_UserTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            this.statuses.Clear();
            foreach (var item in ss)
                this.statuses.Add(item);
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            user = e.NavigationParameter as User;
            this.defaultViewModel["user"] = user;
            this.defaultViewModel["statuses"] = statuses;
            this.defaultViewModel["favorite"] = favorite;
            this.defaultViewModel["friends"] = friends;
            this.defaultViewModel["follower"] = follower;

            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusUserTimeline(user.id, 10);
            FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 10);
            FanfouAPI.FanfouAPI.Instance.UsersFollowers(user.id, 10);
            FanfouAPI.FanfouAPI.Instance.UsersFriends(user.id, 10);
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

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(StatusPage), e.ClickedItem);
        }

        private void statusesGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RepostButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FavButton1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void send_BackClick(object sender, BackClickEventArgs e)
        {

        }

        private void usersGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void usersGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), e.ClickedItem);

        }

        private void hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            if (e.Section == mainHubSection)
            {
                Frame.Navigate(typeof(TimelinePage),
                    new KeyValuePair<TimelinePage.PageType, object>(TimelinePage.PageType.Statuses, this.user));
            }
            else if (e.Section == favHubSection)
            {
                Frame.Navigate(typeof(TimelinePage),
                    new KeyValuePair<TimelinePage.PageType, object>(TimelinePage.PageType.Favorite,this.user));
            }
            else if (e.Section == followHubSection)
            {
                Frame.Navigate(typeof(TimelinePage),
                    new KeyValuePair<TimelinePage.PageType, object>(TimelinePage.PageType.Follower, this.user));
            }
            else if (e.Section == friendHubSection)
            {
                Frame.Navigate(typeof(TimelinePage),
                    new KeyValuePair<TimelinePage.PageType, object>(TimelinePage.PageType.Friends, this.user));
            }
        }
    }
}
