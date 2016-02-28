﻿using System;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouUWP.Common;

using FanfouUWP.Utils;
using Windows.UI.Popups;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace FanfouUWP
{
    /// <summary>
    ///     基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        public LoginPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        ///     可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        /// <summary>
        ///     NavigationHelper 在每页上用于协助导航和
        ///     进程生命期管理
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        /// <summary>
        ///     使用在导航过程中传递的内容填充页。 在从以前的会话
        ///     重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        ///     事件的来源; 通常为 <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     事件数据，其中既提供在最初请求此页时传递给
        ///     <see cref=" Frame.Navigate1(Type, Object)" /> 的导航参数，又提供
        ///     此页在以前会话期间保留的状态的
        ///     的字典。 首次访问页面时，该状态将为 null。
        /// </param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        { }

        /// <summary>
        ///     保留与此页关联的状态，以防挂起应用程序或
        ///     从导航缓存中放弃此页。  值必须符合
        ///     <see cref="SuspensionManager.SessionState" /> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper" /></param>
        /// <param name="e">
        ///     提供要使用可序列化状态填充的空字典
        ///     的事件数据。
        /// </param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private async void SinginButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://fanfou.com/register"));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text.Count() != 0 && password.Password.Count() != 0)
            {
                try
                {
                    await FanfouAPI.FanfouAPI.Instance.Login(username.Text, password.Password);
                    await FanfouAPI.FanfouAPI.Instance.VerifyCredentials();

                    Utils.NavigationControl.ClearStack(this.Frame);
                    Frame.Navigate(typeof(MainPage));
                }
                catch (Exception)
                {
                    var dialog = new MessageDialog("登录失败，请检查用户名或密码是否输入正确，或网络是否畅通", "登录失败");

                    dialog.Commands.Add(new UICommand("确定", cmd => { }, commandId: 0));
                    await dialog.ShowAsync();
                }
            }
        }

        #region NavigationHelper 注册

        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// 
        /// 应将页面特有的逻辑放入用于
        /// <see cref="GridCS.Common.NavigationHelper.LoadState" />
        /// 和
        /// <see cref="GridCS.Common.NavigationHelper.SaveState" />
        /// 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
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