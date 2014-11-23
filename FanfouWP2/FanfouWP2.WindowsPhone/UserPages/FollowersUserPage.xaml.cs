using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI.Events;
using FanfouWP2.FanfouAPI.Items;

namespace FanfouWP2.UserPages
{
    public sealed partial class FollowersUserPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private readonly ObservableCollection<User> users = new ObservableCollection<User>();
        private User user;

        public FollowersUserPage()
        {
            InitializeComponent();

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.UsersFollowersSuccess += Instance_UsersFollowersSuccess;
            FanfouAPI.FanfouAPI.Instance.UsersFollowersFailed += Instance_UsersFollowersFailed;
        }

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        private void Instance_UsersFollowersFailed(object sender, FailedEventArgs e)
        {
        }

        private void Instance_UsersFollowersSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<User>;
            users.Clear();
            foreach (User i in ss)
            {
                users.Add(i);
            }
            defaultViewModel["date"] = DateTime.Now.ToString();
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            defaultViewModel["users"] = users;
            user = e.NavigationParameter as User;

            title.Text = user.screen_name + "的听众";
            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.UsersFollowers(user.id, 60);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.UsersFollowers(user.id, 60);
        }

        private void usersGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof (UserPage), e.ClickedItem);
        }

        #region NavigationHelper 注册

        /// <summary>
        ///     此部分中提供的方法只是用于使
        ///     NavigationHelper 可响应页面的导航方法。
        ///     <para>
        ///         应将页面特有的逻辑放入用于
        ///         <see cref="NavigationHelper.LoadState" />
        ///         和 <see cref="NavigationHelper.SaveState" /> 的事件处理程序中。
        ///         除了在会话期间保留的页面状态之外
        ///         LoadState 方法中还提供导航参数。
        ///     </para>
        /// </summary>
        /// <param name="e">
        ///     提供导航方法数据和
        ///     无法取消导航请求的事件处理程序。
        /// </param>
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