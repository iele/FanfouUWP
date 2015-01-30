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
    public sealed partial class DirectPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private int page = 1;
        private ObservableCollection<DirectMessageItem> messages = new ObservableCollection<DirectMessageItem>();

        public DirectPage()
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
            defaultViewModel["messages"] = messages;

            defaultViewModel["page"] = "第" + page + "页";

            loading.Visibility = Visibility.Visible;
            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(page);
                if (ss.Count() != 0)
                {
                    messages.Clear();
                    foreach (var item in ss)
                    {
                        messages.Add(item);
                    }
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

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
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
                    var ss = await FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(page);
                    if (ss.Count() != 0)
                    {
                        messages.Clear();
                        foreach (var item in ss)
                        {
                            messages.Add(item);
                        }
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
                var ss = await FanfouAPI.FanfouAPI.Instance.DirectMessagesConversationList(page);
                if (ss.Count() != 0)
                {
                    messages.Clear();
                    foreach (var item in ss)
                    {
                        messages.Add(item);
                    }
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
          Frame.Navigate(typeof(MessagePage), Utils.DataConverter<DirectMessageItem>.Convert(e.ClickedItem as DirectMessageItem));
        }
    }
}