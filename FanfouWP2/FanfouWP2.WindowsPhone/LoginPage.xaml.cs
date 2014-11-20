using System;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;
using FanfouWP2.Utils;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace FanfouWP2
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

            FanfouAPI.FanfouAPI.Instance.LoginSuccess += Instance_LoginSuccess;
            FanfouAPI.FanfouAPI.Instance.LoginFailed += Instance_LoginFailed;
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsSuccess += Instance_VerifyCredentialsSuccess;
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsFailed += Instance_VerifyCredentialsFailed;
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


        private void Instance_VerifyCredentialsFailed(object sender, FailedEventArgs e)
        {
            ToastShow.ShowToast("登陆失败", "登录用户名或密码有误");
        }

        private void Instance_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Frame.Navigate(typeof (HomePage));
        }

        private void Instance_LoginFailed(object sender, FailedEventArgs e)
        {
            ToastShow.ShowToast("登陆失败", "登录用户名或密码有误");
        }

        private void Instance_LoginSuccess(object sender, EventArgs e)
        {
            FanfouAPI.FanfouAPI.Instance.VerifyCredentials();
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
        ///     <see cref=" Frame.Navigate(Type, Object)" /> 的导航参数，又提供
        ///     此页在以前会话期间保留的状态的
        ///     的字典。 首次访问页面时，该状态将为 null。
        /// </param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null)
            {
                if (e.PageState["username"] != null)
                    username.Text = e.PageState["username"].ToString();
                if (e.PageState["username"] != null)
                    password.Password = e.PageState["password"].ToString();
            }
        }

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
            e.PageState["username"] = username;
            e.PageState["password"] = password;
        }

        private async void SinginButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://fanfou.com/register"));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text.Count() != 0 && password.Password.Count() != 0)
                FanfouAPI.FanfouAPI.Instance.Login(username.Text, password.Password);
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