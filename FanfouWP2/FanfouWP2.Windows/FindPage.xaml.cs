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
    public sealed partial class FindPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<User> users = new ObservableCollection<User>();

        private string query;
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public FindPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.SearchUserSuccess += Instance_SearchUserSuccess;
            FanfouAPI.FanfouAPI.Instance.SearchUserFailed += Instance_SearchUserFailed;
        }

        void Instance_SearchUserFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_SearchUserSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = (sender as UserList).users;
            users.Clear();
            foreach (var item in ss)
                users.Add(item);
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;

            this.defaultViewModel["data"] = users;
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
            var c = e.ClickedItem as User;
            Frame.Navigate(typeof(UserPage), c);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            query = this.search.Text;
            this.users.Clear();
            this.defaultViewModel["data"] = users;
            FanfouAPI.FanfouAPI.Instance.SearchUser(query, 60);
        }
    }
}
