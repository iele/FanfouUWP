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

        public FindPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.SearchUserSuccess += Instance_SearchUserSuccess;
            FanfouAPI.FanfouAPI.Instance.SearchUserFailed += Instance_SearchUserFailed;
        }

        void Instance_SearchUserFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_SearchUserSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = (sender as UserList).users;
            this.users.Clear();
            foreach (var i in ss)
            {
                this.users.Add(i);
            }
            this.defaultViewModel["date"] = DateTime.Now.ToString();
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel["users"] = users;

            loading.Visibility = Visibility.Collapsed;

            if (e.PageState != null)
            {
                if (e.PageState.ContainsKey("search"))
                    this.search.Text = e.PageState["search"].ToString();
                if (e.PageState.ContainsKey("users"))
                {
                    this.users = e.PageState["users"] as ObservableCollection<User>;
                    this.defaultViewModel["users"] = users;
                }
                return;
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["search"] = search.Text;
            e.PageState["users"] = this.users;
        }

        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        private void SearchItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.SearchUser(this.search.Text, 60);
        }

    }
}
