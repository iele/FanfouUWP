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
    public sealed partial class MessagePage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private ObservableCollection<DirectMessage> messages = new ObservableCollection<DirectMessage>();

        private DirectMessageItem direct;

        public MessagePage()
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
            direct = Utils.DataConverter<DirectMessageItem>.Convert(e.NavigationParameter as string);

            defaultViewModel["messages"] = messages;

            loading.Visibility = Visibility.Visible;
            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.DirectMessagesConversation(direct.otherid);
                ss.Reverse();
                messages.Clear();
                foreach (var item in ss)
                {
                    messages.Add(item);
                }
                if (messages.Count != 0)
                    messagesGridView.ScrollIntoView(messages.Last());
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

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loading.Visibility = Visibility.Visible;
                var ss = await FanfouAPI.FanfouAPI.Instance.DirectMessagesNew(direct.otherid, text.Text);
                messages.Add(ss);
                if (messages.Count != 0)
                    messagesGridView.ScrollIntoView(messages.Last());
                this.text.Text = "";
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
}