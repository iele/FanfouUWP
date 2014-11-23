using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.FanfouAPI.Events;
using FanfouWP2.Utils;

namespace FanfouWP2
{
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public MainPage()
        {
            InitializeComponent();

            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsSuccess += Instance_VerifyCredentialsSuccess;
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsFailed += Instance_VerifyCredentialsFailed;

            Loaded += MainPage_Loaded;
        }

        private void ScenarioFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.  The Parameter
        ///     property is typically used to configure the page.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingStorage.Instance.currentUserAuth == null)
            {
                NavigationControl.ClearStack(Frame);
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
            NavigationControl.ClearStack(Frame);
            Frame.Navigate(typeof (LoginPage));
        }

        private void Instance_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            NavigationControl.ClearStack(Frame);
            Frame.Navigate(typeof (HomePage));
        }
    }
}