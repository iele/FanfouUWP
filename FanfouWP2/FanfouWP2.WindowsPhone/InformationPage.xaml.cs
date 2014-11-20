using System;
using Windows.ApplicationModel.Email;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace FanfouWP2
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class InformationPage : Page
    {
        public InformationPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">
        ///     描述如何访问此页的事件数据。
        ///     此参数通常用于配置页。
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void FollowButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void NoticeButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
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
    }
}