using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.CustomControl;
using FanfouWP2.FanfouAPI;

//“中心页”项模板在 http://go.microsoft.com/fwlink/?LinkId=321224 上有介绍

namespace FanfouWP2
{
    /// <summary>
    ///     显示分组的项集合的页。
    /// </summary>
    public sealed partial class SelfPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        private readonly ObservableCollection<Status> favorite = new ObservableCollection<Status>();
        private readonly ObservableCollection<User> follower = new ObservableCollection<User>();
        private readonly ObservableCollection<User> friends = new ObservableCollection<User>();
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableCollection<Status> statuses = new ObservableCollection<Status>();

        private Status currentClick;
        private User user;

        public SelfPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;

            FanfouAPI.FanfouAPI.Instance.UserTimelineSuccess += Instance_UserTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.UserTimelineFailed += Instance_UserTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouAPI.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;

            FanfouAPI.FanfouAPI.Instance.UsersFollowersSuccess += Instance_UsersFollowersSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFollowersFailed += Instance_UsersFollowersFailed;

            FanfouAPI.FanfouAPI.Instance.UsersFriendsSuccess += Instance_UsersFriendsSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFriendsFailed += Instance_UsersFriendsFailed;

            status.UserButtonClick += status_UserButtonClick;
            status.ReplyButtonClick += status_ReplyButtonClick;
            status.RepostButtonClick += status_RepostButtonClick;
            status.FavButtonClick += status_FavButtonClick;
            status.FavCreateSuccess += status_FavCreateSuccess;
            status.FavDestroySuccess += status_FavDestroySuccess;
        }

        /// <summary>
        ///     可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        private void status_FavDestroySuccess(object sender, EventArgs e)
        {
            var s = sender as Status;
            foreach (Status i in statuses)
            {
                if (i.id == s.id)
                {
                    i.favorited = false;
                }
            }
            foreach (Status i in favorite)
            {
                if (i.id == s.id)
                {
                    i.favorited = false;
                }
            }
        }

        private void status_FavCreateSuccess(object sender, EventArgs e)
        {
            var s = sender as Status;
            foreach (Status i in statuses)
            {
                if (i.id == s.id)
                {
                    i.favorited = true;
                }
            }
            foreach (Status i in favorite)
            {
                if (i.id == s.id)
                {
                    i.favorited = true;
                }
            }
        }

        private void status_FavButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void status_RepostButtonClick(object sender, RoutedEventArgs e)
        {
            sendPopup.IsOpen = true;
            send.ChangeMode(SendSettingsFlyout.SendMode.Repose, currentClick);
        }

        private void status_ReplyButtonClick(object sender, RoutedEventArgs e)
        {
            sendPopup.IsOpen = true;
            send.ChangeMode(SendSettingsFlyout.SendMode.Reply, currentClick);
        }

        private void status_UserButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (UserPage2), currentClick.user);
        }

        private void Instance_UsersFriendsFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_UsersFriendsSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            friends.Clear();
            foreach (User item in ss)
                friends.Add(item);
        }

        private void Instance_UsersFollowersFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_UsersFollowersSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            follower.Clear();
            foreach (User item in ss)
                follower.Add(item);
        }

        private void Instance_FavoritesFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            favorite.Clear();
            foreach (Status item in ss)
                favorite.Add(item);
        }

        private void Instance_UserTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_UserTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            statuses.Clear();
            foreach (Status item in ss)
                statuses.Add(item);
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            user = FanfouAPI.FanfouAPI.Instance.currentUser;
            defaultViewModel["user"] = user;
            defaultViewModel["statuses"] = statuses;
            defaultViewModel["favorite"] = favorite;
            defaultViewModel["friends"] = friends;
            defaultViewModel["follower"] = follower;

            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusUserTimeline(user.id, 10);
            FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 10);
            FanfouAPI.FanfouAPI.Instance.UsersFollowers(user.id, 10);
            FanfouAPI.FanfouAPI.Instance.UsersFriends(user.id, 10);
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            currentClick = e.ClickedItem as Status;
            status.setStatus(currentClick);
            statusPopup.IsOpen = true;
        }

        private void send_BackClick(object sender, BackClickEventArgs e)
        {
            sendPopup.IsOpen = false;
        }

        private void usersGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void usersGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof (UserPage2), e.ClickedItem);
        }

        private void hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            if (e.Section == mainHubSection)
            {
                Frame.Navigate(typeof (TimelinePage),
                    new KeyValuePair<TimelinePage.PageType, object>(TimelinePage.PageType.Statuses, user));
            }
            else if (e.Section == favHubSection)
            {
                Frame.Navigate(typeof (TimelinePage),
                    new KeyValuePair<TimelinePage.PageType, object>(TimelinePage.PageType.Favorite, user));
            }
            else if (e.Section == followHubSection)
            {
                Frame.Navigate(typeof (UserListPage),
                    new KeyValuePair<UserListPage.PageType, object>(UserListPage.PageType.Follower, user));
            }
            else if (e.Section == friendHubSection)
            {
                Frame.Navigate(typeof (UserListPage),
                    new KeyValuePair<UserListPage.PageType, object>(UserListPage.PageType.Friends, user));
            }
        }

        private void status_BackClick(object sender, BackClickEventArgs e)
        {
            statusPopup.IsOpen = false;
        }

        #region NavigationHelper 注册

        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// 
        /// 应将页面特有的逻辑放入用于
        /// <see cref="GridCS.Common.NavigationHelper.LoadState" />
        /// 和
        /// <see cref="GridCS.Common.NavigationHelper.SaveState" />
        /// 的事件处理程序中。
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
    }
}