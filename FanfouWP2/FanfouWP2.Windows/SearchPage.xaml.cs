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
    public sealed partial class SearchPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();

        private Status currentClick;

        private string query;

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public SearchPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.SearchTimelineSuccess += Instance_SearchTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.SearchTimelineFailed += Instance_SearchTimelineFailed;

            this.send.StatusUpdateSuccess += send_StatusUpdateSuccess;
            this.send.StatusUpdateFailed += send_StatusUpdateFailed;

            this.status.UserButtonClick += status_UserButtonClick;
            this.status.ReplyButtonClick += status_ReplyButtonClick;
            this.status.RepostButtonClick += status_RepostButtonClick;
            this.status.FavButtonClick += status_FavButtonClick;
            this.status.FavCreateSuccess += status_FavCreateSuccess;
            this.status.FavDestroySuccess += status_FavDestroySuccess;
        }

        void status_FavDestroySuccess(object sender, EventArgs e)
        {
            var s = sender as Status;
            foreach (var i in statuses)
            {
                if (i.id == s.id)
                {
                    i.favorited = false;
                }
            }
        }
        void status_FavCreateSuccess(object sender, EventArgs e)
        {
            var s = sender as Status;
            foreach (var i in statuses)
            {
                if (i.id == s.id)
                {
                    i.favorited = true;
                }
            }
        }

        void Instance_SearchTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_SearchTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            statuses.Clear();
            foreach (var item in ss)
                statuses.Add(item);
        }

        private void status_FavButtonClick(object sender, RoutedEventArgs e)
        {
        }

        void status_RepostButtonClick(object sender, RoutedEventArgs e)
        {
            this.sendPopup.IsOpen = true;
            this.send.ChangeMode(CustomControl.SendSettingsFlyout.SendMode.Repose, currentClick);
        }

        void status_ReplyButtonClick(object sender, RoutedEventArgs e)
        {
            this.sendPopup.IsOpen = true;
            this.send.ChangeMode(CustomControl.SendSettingsFlyout.SendMode.Reply, currentClick);
        }

        void status_UserButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), currentClick.user);
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
            loading.Visibility = Visibility.Collapsed;

            this.defaultViewModel["data"] = statuses;
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
            currentClick = e.ClickedItem as Status;
            this.status.setStatus(currentClick);
            this.statusPopup.IsOpen = true;
        }

        private void send_BackClick(object sender, BackClickEventArgs e)
        {
            this.sendPopup.IsOpen = false;
        }

        private void status_BackClick(object sender, BackClickEventArgs e)
        {
            this.statusPopup.IsOpen = false;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            query = this.search.Text;
            this.statuses.Clear();
            this.defaultViewModel["data"] = statuses;
            FanfouAPI.FanfouAPI.Instance.SearchTimeline(query, 60);
        }
    }
}
