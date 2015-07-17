using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;

using FanfouWP2.FanfouAPI.Items;
using FanfouWP2.Utils;

namespace FanfouWP2
{
    public sealed partial class DirectPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private int page = 1;
        private PaginatedCollection<DirectMessageItem> messages = new PaginatedCollection<DirectMessageItem>();
        private string title;

        public DirectPage()
        {
            InitializeComponent();

            messages.load = async (c) =>
            {
                try
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(++page);
                    if (result.Count == 0)
                        messages.HasMoreItems = false;

                    foreach (DirectMessageItem i in result)
                    {
                        messages.Add(i);
                    }

                    return result.Count;
                }
                catch (Exception)
                {
                    return 0;
                }
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
            defaultViewModel["messages"] = messages;
            defaultViewModel["title"] = "我的私信";

            try
            {
                page = 1;
                var ss = await FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(page);
                if (ss.Count() != 0)
                {
                    messages.Clear();
                    foreach (var item in ss)
                    {
                        messages.Add(item);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
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

        private void messagesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(MessagePage), (e.ClickedItem as DirectMessageItem).otherid);
        }

        private async void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                page = 1;
                var ss = await FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(page);
                if (ss.Count() != 0)
                {
                    messages.Clear();
                    foreach (var item in ss)
                    {
                        messages.Add(item);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}