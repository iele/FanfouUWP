using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FanfouWP2.CustomControl
{
    public sealed partial class AccountSettingsFlyout : SettingsFlyout
    {
        public AccountSettingsFlyout()
        {
            this.InitializeComponent();

            if (FanfouAPI.FanfouAPI.Instance.currentUser != null)
                this.DataContext = FanfouAPI.FanfouAPI.Instance.currentUser;
            else
            {
                text.Text = "尚未登录";
                name.Visibility = Visibility.Collapsed;
                logout.Visibility = Visibility.Collapsed;
            }
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            FanfouAPI.FanfouAPI.Instance.Logout();
            text.Text = "尚未登录";
            name.Visibility = Visibility.Collapsed;
            logout.Visibility = Visibility.Collapsed;
            var rootFrame = new Frame();
            Window.Current.Content = rootFrame;
            rootFrame.Navigate(typeof(LoginPage));
        }
    }
}
