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
    public sealed partial class TimelinePage : Page
    {
        public enum PageType
        {
            Statuses,
            Favorite
        };

        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private readonly ObservableCollection<ObservableCollection<Status>> statuses =
            new ObservableCollection<ObservableCollection<Status>>();

        private Status currentClick;

        private PageType currentType;
        private object data;

        public TimelinePage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.UserTimelineSuccess += Instance_UserTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.UserTimelineFailed += Instance_UserTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouAPI.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;

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
            foreach (var ss in statuses)
            {
                foreach (Status i in ss)
                {
                    if (i.id == s.id)
                    {
                        i.favorited = false;
                    }
                }
            }
        }

        private void status_FavCreateSuccess(object sender, EventArgs e)
        {
            var s = sender as Status;
            foreach (var ss in statuses)
            {
                foreach (Status i in ss)
                {
                    if (i.id == s.id)
                    {
                        i.favorited = true;
                    }
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
            Frame.Navigate(typeof (UserPage), currentClick.user);
        }

        private void Instance_FavoritesFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            if (ss.Count != 0)
                statuses.Add(new ObservableCollection<Status>(ss));
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

        private void Instance_UserTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_UserTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            if (ss.Count != 0)
                statuses.Add(new ObservableCollection<Status>(ss));
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            currentType = ((KeyValuePair<PageType, object>) e.NavigationParameter).Key;
            data = ((KeyValuePair<PageType, object>) e.NavigationParameter).Value;

            loading.Visibility = Visibility.Visible;

            defaultViewModel["statuses"] = statuses;

            switch (currentType)
            {
                case PageType.Statuses:
                    FanfouAPI.FanfouAPI.Instance.StatusUserTimeline((data as User).id, 60);
                    if ((data as User).id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                        defaultViewModel["title"] = "我的消息";
                    else
                        defaultViewModel["title"] = (data as User).screen_name + "的消息";
                    break;
                case PageType.Favorite:
                    FanfouAPI.FanfouAPI.Instance.FavoritesId((data as User).id, 60, 1);
                    if ((data as User).id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                        defaultViewModel["title"] = "我的收藏";
                    else
                        defaultViewModel["title"] = (data as User).screen_name + "的收藏";
                    break;
                default:
                    break;
            }

            defaultViewModel["page"] = "第1页";
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

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            defaultViewModel["page"] = "第" + (flipView.SelectedIndex + 1) + "页";

            if (flipView.SelectedIndex == flipView.Items.Count() - 1 && flipView.Items.Count > 0)
            {
                loading.Visibility = Visibility.Visible;
                switch (currentType)
                {
                    case PageType.Statuses:
                        FanfouAPI.FanfouAPI.Instance.StatusUserTimeline((data as User).id, 60,
                            flipView.Items.Count() + 1);
                        break;
                    case PageType.Favorite:
                        FanfouAPI.FanfouAPI.Instance.FavoritesId(FanfouAPI.FanfouAPI.Instance.currentUser.id, 60,
                            flipView.Items.Count() + 1);
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

        private void send_BackClick(object sender, BackClickEventArgs e)
        {
            sendPopup.IsOpen = false;
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