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
    public sealed partial class FavoritePage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private int page = 1;
        private User user;
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();

        public FavoritePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouAPI.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;
        }

        void Instance_FavoritesFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            if (ss.Count() != 0)
            {
                this.statuses.Clear();
                Utils.StatusesReform.reform(this.statuses, ss);
                this.defaultViewModel["page"] = "第" + page + "页";
                changeMenu(false);
            }
            else
            {
                changeMenu(true);
            }
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
            user = e.NavigationParameter as User;

            if (e.PageState != null)
            {
                if (e.PageState["page"] != null)
                    page = (int)e.PageState["page"];
                if (e.PageState["user"] != null)
                    user = e.PageState["user"] as User;
                if (e.PageState["statuses"] != null)
                    statuses = e.PageState["statuses"] as ObservableCollection<Status>;
                if (e.PageState["PrevItem.IsEnabled"] != null)
                    PrevItem.IsEnabled = (bool)e.PageState["PrevItem.IsEnabled"];
                if (e.PageState["NextItem.IsEnabled"] != null)
                    NextItem.IsEnabled = (bool)e.PageState["NextItem.IsEnabled"];
            }

            this.defaultViewModel["statuses"] = statuses;

            if (user.id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                this.defaultViewModel["title"] = "我的收藏";
            else
                this.defaultViewModel["title"] = user.screen_name + "的收藏";

            this.defaultViewModel["page"] = "第" + page + "页";

            loading.Visibility = Visibility.Collapsed;

            if (e.PageState == null)
            {
                loading.Visibility = Visibility.Visible;
                FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 60, 1);
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["page"] = page;
            e.PageState["user"] = user;
            e.PageState["statuses"] = statuses;
            e.PageState["PrevItem.IsEnabled"] = PrevItem.IsEnabled;
            e.PageState["NextItem.IsEnabled"] = NextItem.IsEnabled;
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


        private void PrevItem_Click(object sender, RoutedEventArgs e)
        {
            this.loading.Visibility = Visibility.Visible;
            if (page >= 1)
            {
                page--;
                FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 60, page);
                changeMenu(false, true);
            }
        }

        private void NextItem_Click(object sender, RoutedEventArgs e)
        {
            this.loading.Visibility = Visibility.Visible;
            page++;
            FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 60, page);
            changeMenu(false, true);
        }

        private void changeMenu(bool is_end, bool is_disabled = false)
        {
            if (page <= 1 || is_disabled)
                PrevItem.IsEnabled = false;
            else
                PrevItem.IsEnabled = true;

            if (is_end || is_disabled)
                NextItem.IsEnabled = false;
            else
                NextItem.IsEnabled = true;
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(StatusPage), e.ClickedItem);
        }
    }
}
