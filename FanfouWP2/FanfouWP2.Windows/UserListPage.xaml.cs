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
    public sealed partial class UserListPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<ObservableCollection<User>> users = new ObservableCollection<ObservableCollection<User>>();

        public enum PageType { Follower, Friends };
        private PageType currentType;
        private object data;
        private Status currentSelection;

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public UserListPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.UsersFriendsSuccess += Instance_UsersFriendsSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFriendsFailed += Instance_UsersFriendsFailed;

            FanfouAPI.FanfouAPI.Instance.UsersFollowersSuccess += Instance_UsersFollowersSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFollowersFailed += Instance_UsersFollowersFailed;

            this.send.StatusUpdateSuccess += send_StatusUpdateSuccess;
            this.send.StatusUpdateFailed += send_StatusUpdateFailed;
        }

        void Instance_UsersFollowersFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_UsersFollowersSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            if (ss.Count != 0)
                users.Add(new ObservableCollection<User>(ss));
        }

        void Instance_UsersFriendsFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_UsersFriendsSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            if (ss.Count != 0)
                users.Add(new ObservableCollection<User>(ss));
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
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            currentType = ((KeyValuePair<PageType, object>)e.NavigationParameter).Key;
            data = ((KeyValuePair<PageType, object>)e.NavigationParameter).Value;

            loading.Visibility = Visibility.Visible;
            this.defaultViewModel["users"] = users;

            switch (currentType)
            {
                case PageType.Follower:
                    FanfouAPI.FanfouAPI.Instance.UsersFollowers((data as User).id, 60, 1);
                    if ((data as User).id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                        this.defaultViewModel["title"] = "我的听众";
                    else
                        this.defaultViewModel["title"] = (data as User).screen_name + "的听众";
                    break;
                case PageType.Friends:
                    FanfouAPI.FanfouAPI.Instance.UsersFriends((data as User).id, 60, 1);
                    if ((data as User).id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                        this.defaultViewModel["title"] = "我的好友";
                    else
                        this.defaultViewModel["title"] = (data as User).screen_name + "的好友";
                    break;
                default:
                    break;
            }

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
            var item = e.ClickedItem as Status;
            Frame.Navigate(typeof(StatusPage), item);
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.defaultViewModel["page"] = "第" + (this.flipView.SelectedIndex + 1).ToString() + "页";

            if (this.flipView.SelectedIndex == this.flipView.Items.Count() - 1)
            {
                loading.Visibility = Visibility.Visible;
                switch (currentType)
                {
                    case PageType.Follower:
                        FanfouAPI.FanfouAPI.Instance.UsersFollowers((data as User).id, 60, this.flipView.Items.Count() + 1);
                        break;
                    case PageType.Friends:
                        FanfouAPI.FanfouAPI.Instance.UsersFriends((data as User).id, 60, this.flipView.Items.Count() + 1);
                        break;
                    default:
                        break;
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.users.Clear();
            loading.Visibility = Visibility.Visible;

            switch (currentType)
            {
                case PageType.Follower:
                    FanfouAPI.FanfouAPI.Instance.UsersFollowers((data as User).id, 60, 1);
                    if ((data as User).id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                        this.defaultViewModel["title"] = "我的听众";
                    else
                        this.defaultViewModel["title"] = (data as User).screen_name + "的听众";
                    break;
                case PageType.Friends:
                    FanfouAPI.FanfouAPI.Instance.UsersFriends((data as User).id, 60, 1);
                    if ((data as User).id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                        this.defaultViewModel["title"] = "我的好友";
                    else
                        this.defaultViewModel["title"] = (data as User).screen_name + "的好友";
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
