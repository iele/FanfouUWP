using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FanfouWP.Storage;
using FanfouWP2.FanfouAPI;

namespace FanfouWP2
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsSuccess += Instance_VerifyCredentialsSuccess;
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsFailed += Instance_VerifyCredentialsFailed;

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingStorage.Instance.currentUserAuth == null)
            {
                Frame.Navigate(typeof (LoginPage));
            }
            else
            {
                FanfouAPI.FanfouAPI.Instance.setUserAuth(SettingStorage.Instance.currentUserAuth);
                FanfouAPI.FanfouAPI.Instance.VerifyCredentials();
            }
        }

        private void Instance_VerifyCredentialsFailed(object sender, FailedEventArgs e)
        {
            Frame.Navigate(typeof (LoginPage));
        }

        private void Instance_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Frame.Navigate(typeof (HomePage));
        }
    }
}