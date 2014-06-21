using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FanfouWP2
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            FanfouAPI.FanfouAPI.Instance.LoginSuccess += Instance_LoginSuccess;
            FanfouAPI.FanfouAPI.Instance.LoginFailed += Instance_LoginFailed;
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsSuccess += Instance_VerifyCredentialsSuccess;
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsFailed += Instance_VerifyCredentialsFailed;
        }

        void Instance_VerifyCredentialsFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }

        void Instance_LoginFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_LoginSuccess(object sender, EventArgs e)
        {
            FanfouAPI.FanfouAPI.Instance.VerifyCredentials();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FanfouAPI.FanfouAPI.Instance.Login("elephas", "myelephas");
        }
    }
}
