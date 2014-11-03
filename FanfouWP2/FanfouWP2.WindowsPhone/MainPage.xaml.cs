using FanfouWP.Storage;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Core;
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

        /// <summary> 
        /// Invoked when this page is about to be displayed in a Frame. 
        /// </summary> 
        /// <param name="e">Event data that describes how this page was reached.  The Parameter 
        /// property is typically used to configure the page.</param> 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingStorage.Instance.currentUserAuth == null)
            {
                Utils.NavigationControl.ClearStack(Frame);
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
            Utils.NavigationControl.ClearStack(Frame);
            Frame.Navigate(typeof(LoginPage));
        }

        void Instance_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Utils.NavigationControl.ClearStack(Frame);
            Frame.Navigate(typeof(HomePage));
        }
    }
}
