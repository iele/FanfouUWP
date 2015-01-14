using System;
using Windows.ApplicationModel.Email;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI.Items;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace FanfouWP2
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class InformationPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        public InformationPage()
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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
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

        private void FollowButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void NoticeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AnnouncePage));
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.ReplyUser).ToString() + Utils.DataConverter<User>.Convert(new User() { id = "fanwp", screen_name = "饭窗" }));
        }

        private void MarketButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new EmailMessage();
            message.To.Add(new EmailRecipient("melephas@outlook.com"));
            message.Subject = "关于饭窗WP8.1的意见";
            await EmailManager.ShowComposeNewEmailAsync(message);
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