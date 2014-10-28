using FanfouWP.Storage;
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

            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsSuccess += Instance_VerifyCredentialsSuccess;
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsFailed += Instance_VerifyCredentialsFailed;

            this.Loaded += MainPage_Loaded;            
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingStorage.Instance.currentUserAuth == null)
            {
                Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                FanfouAPI.FanfouAPI.Instance.setUserAuth(SettingStorage.Instance.currentUserAuth);
                FanfouAPI.FanfouAPI.Instance.VerifyCredentials();
            }
        }

        void Instance_VerifyCredentialsFailed(object sender, FailedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        void Instance_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }
    }
}
