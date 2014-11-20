using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.CustomControl;
using FanfouWP2.FanfouAPI;

namespace FanfouWP2
{
    public sealed partial class SearchPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableCollection<Status> statuses = new ObservableCollection<Status>();

        private Status currentClick;

        private string query;

        public SearchPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.SearchTimelineSuccess += Instance_SearchTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.SearchTimelineFailed += Instance_SearchTimelineFailed;

            send.StatusUpdateSuccess += send_StatusUpdateSuccess;
            send.StatusUpdateFailed += send_StatusUpdateFailed;

            status.UserButtonClick += status_UserButtonClick;
            status.ReplyButtonClick += status_ReplyButtonClick;
            status.RepostButtonClick += status_RepostButtonClick;
            status.FavButtonClick += status_FavButtonClick;
            status.FavCreateSuccess += status_FavCreateSuccess;
            status.FavDestroySuccess += status_FavDestroySuccess;
        }

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
        }

        private void Instance_SearchTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_SearchTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            statuses.Clear();
            foreach (Status item in ss)
                statuses.Add(item);
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
            Frame.Navigate(typeof (UserPage), currentClick.user);
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
            loading.Visibility = Visibility.Collapsed;

            defaultViewModel["data"] = statuses;
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
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

        private void status_BackClick(object sender, BackClickEventArgs e)
        {
            statusPopup.IsOpen = false;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            query = search.Text;
            statuses.Clear();
            defaultViewModel["data"] = statuses;
            FanfouAPI.FanfouAPI.Instance.SearchTimeline(query, 60);
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