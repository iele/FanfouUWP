using FanfouWP2.FanfouAPI.Items;
using FanfouWP2.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FanfouWP2
{
    public sealed partial class FramePage : Page
    {
        private User currentUser { get; set; } = SettingStorage.Instance.currentUser;

        public FramePage()
        {
            this.InitializeComponent();

            TitleBar.setTitleBar();

            Loaded += FramePage_Loaded;
        }

        private void FramePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                MainFrame.Navigate(typeof(LoginPage));
            }
            else
            {
                FanfouAPI.FanfouAPI.Instance.setUser(currentUser);
                MainFrame.Navigate(typeof(MainPage));
            }
        }
    }
}
