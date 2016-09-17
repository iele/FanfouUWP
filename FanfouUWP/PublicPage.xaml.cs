using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouUWP.Common;

using FanfouUWP.FanfouAPI.Items;
using FanfouUWP.Utils;

namespace FanfouUWP
{
    public sealed partial class PublicPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private readonly ObservableCollection<Status> statuses = new ObservableCollection<Status>();

        public PublicPage()
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
            defaultViewModel["statuses"] = statuses;

            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(SettingStorage.Instance.messageSize, 1);
                statuses.Clear();
                StatusesReform.append(statuses, ss);
                defaultViewModel["date"] = DateTime.Now.ToString();
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }

        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private async void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(SettingStorage.Instance.messageSize, 1);
                statuses.Clear();
                StatusesReform.append(statuses, ss);
                defaultViewModel["date"] = DateTime.Now.ToString();
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }
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