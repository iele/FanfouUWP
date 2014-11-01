using FanfouWP.Storage;
using FanfouWP2.Common;
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
        public static MainPage Current;     
        public MainPage()
        {
            this.InitializeComponent();

            Current = this;             
 
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsSuccess += Instance_VerifyCredentialsSuccess;
            FanfouAPI.FanfouAPI.Instance.VerifyCredentialsFailed += Instance_VerifyCredentialsFailed;

            this.Loaded += MainPage_Loaded;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        /// <summary> 
        /// Invoked when this page is about to be displayed in a Frame. 
        /// </summary> 
        /// <param name="e">Event data that describes how this page was reached.  The Parameter 
        /// property is typically used to configure the page.</param> 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SuspensionManager.RegisterFrame(ScenarioFrame, "scenarioFrame");
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (ScenarioFrame.CanGoBack)
            {
                ScenarioFrame.GoBack();

                e.Handled = true;
            }
        } 


        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingStorage.Instance.currentUserAuth == null)
            {
                Utils.NavigationControl.ClearStack(Frame);
                ScenarioFrame.Navigate(typeof(LoginPage));
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
            ScenarioFrame.Navigate(typeof(LoginPage));
        }

        void Instance_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Utils.NavigationControl.ClearStack(Frame);
            ScenarioFrame.Navigate(typeof(HomePage));
        }
    }
}
