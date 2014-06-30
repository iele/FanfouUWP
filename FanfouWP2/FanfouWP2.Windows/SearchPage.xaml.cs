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
        private ObservableCollection<ObservableCollection<Status>> statuses = new ObservableCollection<ObservableCollection<Status>>();
        private ObservableCollection<ObservableCollection<User>> users = new ObservableCollection<ObservableCollection<User>>();

        private Status currentClick;

        private enum PageType { Timeline, User };

        private PageType currentType;

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

            this.send.StatusUpdateSuccess += send_StatusUpdateSuccess;
            this.send.StatusUpdateFailed += send_StatusUpdateFailed;

            this.status.UserButtonClick += status_UserButtonClick;
            this.status.ReplyButtonClick += status_ReplyButtonClick;
            this.status.RepostButtonClick += status_RepostButtonClick;
            this.status.FavButtonClick += status_FavButtonClick;
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

        void Instance_FavoritesFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
        }

        void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            if (ss.Count != 0)
                statuses.Add(new ObservableCollection<Status>(ss));
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
            loading.Visibility = Visibility.Visible;

            this.defaultViewModel["statuses"] = statuses;
            this.defaultViewModel["page"] = "第1页";
            this.type.ItemsSource= new string[2]{"搜索时间线", "搜索用户"};
            this.type.SelectedIndex = 0;
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

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.defaultViewModel["page"] = "第" + (this.flipView.SelectedIndex + 1).ToString() + "页";

            if (this.flipView.SelectedIndex == this.flipView.Items.Count() - 1)
            {
                loading.Visibility = Visibility.Visible;
                switch (currentType)
                {
                    case PageType.Timeline:
                        break;
                    case PageType.User:
                        break;
                    default:
                        break;
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.statuses.Clear();
            loading.Visibility = Visibility.Visible;

            switch (currentType)
            {
                case PageType.Timeline:
                    break;
                case PageType.User:
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

        private void send_BackClick(object sender, BackClickEventArgs e)
        {
            this.sendPopup.IsOpen = false;
        }

        private void status_BackClick(object sender, BackClickEventArgs e)
        {
            this.statusPopup.IsOpen = false;
        }
    }
}
