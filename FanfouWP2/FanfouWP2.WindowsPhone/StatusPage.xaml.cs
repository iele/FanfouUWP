using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI.Events;
using FanfouWP2.FanfouAPI.Items;
using FanfouWP2.ItemControl;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace FanfouWP2
{
    /// <summary>
    ///     可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StatusPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private Status status;

        public StatusPage()
        {
            InitializeComponent();

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;
        }

        /// <summary>
        ///     获取与此 <see cref="Page" /> 关联的 <see cref="NavigationHelper" />。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        /// <summary>
        ///     获取此 <see cref="Page" /> 的视图模型。
        ///     可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        /// <summary>
        ///     使用在导航过程中传递的内容填充页。  在从以前的会话
        ///     重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        ///     事件的来源; 通常为 <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     事件数据，其中既提供在最初请求此页时传递给
        ///     <see cref=" Frame.Navigate1(Type, Object)" /> 的导航参数，又提供
        ///     此页在以前会话期间保留的状态的
        ///     字典。 首次访问页面时，该状态将为 null。
        /// </param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            status = Utils.DataConverter<Status>.Convert(e.NavigationParameter as string);
            defaultViewModel["status"] = status;

            loading.Visibility = Visibility.Collapsed;

            if (this.status.favorited)
            {
                this.FavItem.Label = "取消收藏";
                this.FavItem.Icon = new SymbolIcon(Symbol.UnFavorite);
            }
            else
            {
                this.FavItem.Label = "收藏";
                this.FavItem.Icon = new SymbolIcon(Symbol.Favorite);
            }

            if (this.status.user.id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
            {
                this.DeleteItem.Visibility = Visibility.Visible;
                this.DeleteItem.IsEnabled = true;
            }
            else
            {
                this.DeleteItem.Visibility = Visibility.Collapsed;
                this.DeleteItem.IsEnabled = false;
            }

            if (status.in_reply_to_status_id != null && status.in_reply_to_status_id != "")
            {
                loading.Visibility = Visibility.Visible;
                try
                {
                    var ss = await FanfouAPI.FanfouAPI.Instance.StatusContextTimeline(status.id);
                    foreach (Status item in ss)
                    {
                        var sic = new StatusItemControl();
                        sic.DataContext = item;
                        sic.Margin = new Thickness(0, 6, 0, 0);
                        context.Children.Add(sic);
                    }
                    context.Visibility = Visibility.Visible;
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

        /// <summary>
        ///     保留与此页关联的状态，以防挂起应用程序或
        ///     从导航缓存中放弃此页。值必须符合
        ///     <see cref="SuspensionManager.SessionState" /> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper" /></param>
        /// <param name="e">
        ///     提供要使用可序列化状态填充的空字典
        ///     的事件数据。
        /// </param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void RepostItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage), new Tuple<Status, SendPage.SendMode>(status, SendPage.SendMode.Repost));
        }

        private void UserItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(status.user));
        }

        private async void FavItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            try
            {
                if (!this.status.favorited)
                {
                    status = await FanfouAPI.FanfouAPI.Instance.FavoritesCreate(this.status.id);
                    defaultViewModel["status"] = status;
                    this.FavItem.Label = "取消收藏";
                    this.FavItem.Icon = new SymbolIcon(Symbol.UnFavorite);

                    Utils.TimelineCache.Instance.FindAndChange(status);
                }

                else
                {
                    status = await FanfouAPI.FanfouAPI.Instance.FavoritesDestroy(this.status.id);
                    defaultViewModel["status"] = status;
                    this.FavItem.Label = "收藏";
                    this.FavItem.Icon = new SymbolIcon(Symbol.Favorite);
                    Utils.TimelineCache.Instance.FindAndChange(status);
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

        private void ReplyItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage), new Tuple<Status, SendPage.SendMode>(status, SendPage.SendMode.Reply));
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImagePage), status.photo.largeurl);
        }

        private void Profile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(status.user));
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

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            try
            {
                var result = await FanfouAPI.FanfouAPI.Instance.StatusDestroy(this.status.id);
                Utils.TimelineCache.Instance.FindAndDelete(this.status);
                navigationHelper.GoBack();
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                loading.Visibility = Visibility.Collapsed;
            }
        }
    }
}