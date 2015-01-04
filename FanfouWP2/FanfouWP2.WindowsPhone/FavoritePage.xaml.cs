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

namespace FanfouWP2
{
    public sealed partial class FavoritePage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private int page = 1;
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private User user;

        public FavoritePage()
        {
            InitializeComponent();

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

            defaultViewModel["statuses"] = statuses;

            if (user.id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
                defaultViewModel["title"] = "我的收藏";
            else
                defaultViewModel["title"] = user.screen_name + "的收藏";

            defaultViewModel["page"] = "第" + page + "页";

            loading.Visibility = Visibility.Collapsed;

            if (e.PageState == null)
            {
                loading.Visibility = Visibility.Visible;
                try
                {
                    var ss = await FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 60, 1);
                    if (ss.Count() != 0)
                    {
                        statuses.Clear();
                        StatusesReform.append(statuses, ss);
                        defaultViewModel["page"] = "第" + page + "页";
                        changeMenu(false);
                    }
                    else
                    {
                        changeMenu(true);
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    loading.Visibility = Visibility.Collapsed;
                }
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

        private async void PrevItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            if (page >= 1)
            {
                page--;

                loading.Visibility = Visibility.Visible;
                try
                {
                    changeMenu(false, true);
                    var ss = await FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 60, page);
                    if (ss.Count() != 0)
                    {
                        statuses.Clear();
                        StatusesReform.append(statuses, ss);
                        defaultViewModel["page"] = "第" + page + "页";
                        changeMenu(false);
                    }
                    else
                    {
                        changeMenu(true);
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    loading.Visibility = Visibility.Collapsed;
                }              
            }
        }

        private async void NextItem_Click(object sender, RoutedEventArgs e)
        {
            page++;

            loading.Visibility = Visibility.Visible;
            try
            {
                changeMenu(false, true);
                var ss = await FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 60, page);
                if (ss.Count() != 0)
                {
                    statuses.Clear();
                    StatusesReform.append(statuses, ss);
                    defaultViewModel["page"] = "第" + page + "页";
                    changeMenu(false);
                }
                else
                {
                    changeMenu(true);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                loading.Visibility = Visibility.Collapsed;
            }                          
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
            Frame.Navigate(typeof(StatusPage), Utils.DataConverter<Status>.Convert(e.ClickedItem as Status));
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