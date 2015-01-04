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
    public sealed partial class FindPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private ObservableCollection<User> users = new ObservableCollection<User>();

        public FindPage()
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

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            defaultViewModel["users"] = users;

            loading.Visibility = Visibility.Collapsed;

            if (e.PageState != null)
            {
                if (e.PageState.ContainsKey("search"))
                    search.Text = e.PageState["search"].ToString();
                if (e.PageState.ContainsKey("users"))
                {
                    users = e.PageState["users"] as ObservableCollection<User>;
                    defaultViewModel["users"] = users;
                }
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["search"] = search.Text;
            e.PageState["users"] = users;
        }

        private async void SearchItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            var ss = await FanfouAPI.FanfouAPI.Instance.SearchUser(search.Text, 60);

            loading.Visibility = Visibility.Collapsed;
            users.Clear();
            foreach (User i in ss.users)
            {
                users.Add(i);
            }
            defaultViewModel["date"] = DateTime.Now.ToString();
        }

        private void userGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert((e.ClickedItem as User)));
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