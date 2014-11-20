using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;

namespace FanfouWP2
{
    public sealed partial class FindPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableCollection<User> users = new ObservableCollection<User>();

        private string query;

        public FindPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.SearchUserSuccess += Instance_SearchUserSuccess;
            FanfouAPI.FanfouAPI.Instance.SearchUserFailed += Instance_SearchUserFailed;
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }


        private void Instance_SearchUserFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_SearchUserSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            ObservableCollection<User> ss = (sender as UserList).users;
            users.Clear();
            foreach (User item in ss)
                users.Add(item);
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;

            defaultViewModel["data"] = users;
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var c = e.ClickedItem as User;
            Frame.Navigate(typeof (UserPage), c);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            query = search.Text;
            users.Clear();
            defaultViewModel["data"] = users;
            FanfouAPI.FanfouAPI.Instance.SearchUser(query, 60);
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