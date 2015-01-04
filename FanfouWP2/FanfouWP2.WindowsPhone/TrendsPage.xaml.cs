using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI.Events;
using FanfouWP2.FanfouAPI.Items;

namespace FanfouWP2
{
    public sealed partial class TrendsPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private ObservableCollection<Trends> trends = new ObservableCollection<Trends>();

        public TrendsPage()
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
            defaultViewModel["trends"] = trends;
            loading.Visibility = Visibility.Visible;

            if (e.PageState != null && e.PageState["trends"] != null)
            {
                loading.Visibility = Visibility.Collapsed;
                trends = e.PageState["trends"] as ObservableCollection<Trends>;
                defaultViewModel["trends"] = trends;
                return;
            }

            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.TrendsList();
                trends.Clear();
                foreach (Trends item in ss.trends)
                {
                    trends.Add(item);
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

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["trends"] = trends;
        }

        private void trendsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trendsGridView.SelectedIndex != -1)
            {
                var t = trendsGridView.SelectedItem as Trends;
                Frame.Navigate1(typeof (SearchPage), t);
            }
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