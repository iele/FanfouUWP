using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI.Events;
using FanfouWP2.FanfouAPI.Items;
using FanfouWP2.Utils;

namespace FanfouWP2.UserPages
{
    public sealed partial class ImageUserPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private readonly PaginatedCollection<Status> statuses = new PaginatedCollection<Status>();
        private User user;

        public ImageUserPage()
        {
            InitializeComponent();

            statuses.load = async (c) =>
            {
                if (statuses.Count > 0)
                {
                    loading.Visibility = Visibility.Visible;
                    var list = await FanfouAPI.FanfouAPI.Instance.PhotosUserTimeline(user.id, c, max_id: statuses.Last().id);
                    loading.Visibility = Visibility.Collapsed;
                    if (list.Count == 0)
                        statuses.HasMoreItems = false; 
                    Utils.StatusesReform.append(statuses, list);
                    return list.Count;
                }
                return 0;
            };
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;
        }


        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            defaultViewModel["statuses"] = statuses;
            user = e.NavigationParameter as User;

            title.Text = user.screen_name + "的图片";
            loading.Visibility = Visibility.Visible;
            var list = await FanfouAPI.FanfouAPI.Instance.PhotosUserTimeline(user.id, 60);
            loading.Visibility = Visibility.Collapsed;
            Utils.StatusesReform.append(statuses, list);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private async void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            var list = await FanfouAPI.FanfouAPI.Instance.PhotosUserTimeline(user.id, 60);
            loading.Visibility = Visibility.Collapsed;
            Utils.StatusesReform.append(statuses, list);
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(StatusPage), e.ClickedItem);
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