using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.CustomControl;
using FanfouWP2.FanfouAPI;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace FanfouWP2
{
    public sealed partial class UserListPage : Page
    {
        public enum PageType
        {
            Follower,
            Friends
        };

        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private readonly ObservableCollection<ObservableCollection<User>> users =
            new ObservableCollection<ObservableCollection<User>>();

        private Status currentSelection;

        private PageType currentType;
        private object data;

        public UserListPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.UsersFriendsSuccess += Instance_UsersFriendsSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFriendsFailed += Instance_UsersFriendsFailed;

            FanfouAPI.FanfouAPI.Instance.UsersFollowersSuccess += Instance_UsersFollowersSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFollowersFailed += Instance_UsersFollowersFailed;

            send.StatusUpdateSuccess += send_StatusUpdateSuccess;
            send.StatusUpdateFailed += send_StatusUpdateFailed;
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }


        private void Instance_UsersFollowersFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_UsersFollowersSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            if (ss.Count != 0)
                users.Add(new ObservableCollection<User>(ss));
        }

        private void Instance_UsersFriendsFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_UsersFriendsSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            if (ss.Count != 0)
                users.Add(new ObservableCollection<User>(ss));
        }

        private void send_StatusUpdateFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void send_StatusUpdateSuccess(object sender, EventArgs e)
        {
            sendPopup.IsOpen = false;
            loading.Visibility = Visibility.Visible;
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            currentType = ((KeyValuePair<PageType, object>) e.NavigationParameter).Key;
            data = ((KeyValuePair<PageType, object>) e.NavigationParameter).Value;

            loading.Visibility = Visibility.Visible;
            defaultViewModel["users"] = users;

            switch (currentType)
            {
                case PageType.Follower:
                    FanfouAPI.FanfouAPI.Instance.UsersFollowers((data as User).id, 60, 1);
                    if ((data as User).id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                        defaultViewModel["title"] = "我的听众";
                    else
                        defaultViewModel["title"] = (data as User).screen_name + "的听众";
                    break;
                case PageType.Friends:
                    FanfouAPI.FanfouAPI.Instance.UsersFriends((data as User).id, 60, 1);
                    if ((data as User).id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                        defaultViewModel["title"] = "我的好友";
                    else
                        defaultViewModel["title"] = (data as User).screen_name + "的好友";
                    break;
                default:
                    break;
            }

            defaultViewModel["page"] = "第1页";
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            defaultViewModel["page"] = "第" + (flipView.SelectedIndex + 1) + "页";

            if (flipView.SelectedIndex == flipView.Items.Count() - 1 && flipView.Items.Count > 0)
            {
                loading.Visibility = Visibility.Visible;
                switch (currentType)
                {
                    case PageType.Follower:
                        FanfouAPI.FanfouAPI.Instance.UsersFollowers((data as User).id, 60, flipView.Items.Count() + 1);
                        break;
                    case PageType.Friends:
                        FanfouAPI.FanfouAPI.Instance.UsersFriends((data as User).id, 60, flipView.Items.Count() + 1);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (flipView.SelectedIndex > 0)
                flipView.SelectedIndex--;
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (flipView.SelectedIndex < flipView.Items.Count() - 1)
                flipView.SelectedIndex++;
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
            sendPopup.IsOpen = true;
            send.ChangeMode(SendSettingsFlyout.SendMode.Reply, currentSelection);
        }

        private void send_BackClick(object sender, BackClickEventArgs e)
        {
            sendPopup.IsOpen = false;
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (UserPage), currentSelection.user);
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
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