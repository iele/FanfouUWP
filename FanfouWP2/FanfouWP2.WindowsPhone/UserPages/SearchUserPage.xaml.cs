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
    public sealed partial class SearchUserPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private User user;

        public SearchUserPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.SearchUserTimelineSuccess += Instance_SearchUserTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.SearchUserTimelineFailed += Instance_SearchUserTimelineFailed;
        }

        private void Instance_SearchUserTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            this.statuses.Clear();
            Utils.StatusesReform.reform(this.statuses, ss);
            this.defaultViewModel["date"] = DateTime.Now.ToString();
        }

        private void Instance_SearchUserTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
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
            this.defaultViewModel["statuses"] = statuses;
            loading.Visibility = Visibility.Collapsed;

            if (e.PageState != null)
            {
                if (e.PageState.ContainsKey("search"))
                    this.search.Text = e.PageState["search"].ToString();
                if (e.PageState.ContainsKey("statuses"))
                {
                    this.statuses = e.PageState["statuses"] as ObservableCollection<Status>;
                    this.defaultViewModel["statuses"] = statuses;
                }
                if (e.PageState.ContainsKey("user"))
                    this.user = e.PageState["user"] as User;
            }

            var t = e.NavigationParameter as User;
            this.user = t;
            this.title.Text = "搜索 " + this.user.screen_name + " 的时间线";
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["search"] = search.Text;
            e.PageState["statuses"] = this.statuses;
            e.PageState["user"] = this.user;
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
            FanfouAPI.FanfouAPI.Instance.SearchUserTimeline(search.Text, user.id, 60);
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(StatusPage), e.ClickedItem);
        }
    }
}
