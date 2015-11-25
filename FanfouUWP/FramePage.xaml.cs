using FanfouUWP.CustomControl;
using FanfouUWP.FanfouAPI.Items;
using FanfouUWP.Utils;
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

namespace FanfouUWP
{
    public sealed partial class FramePage : Page
    {
        private User currentUser { get; set; } = SettingStorage.Instance.currentUser;

        public FramePage()
        {
            this.InitializeComponent();

            App.RootFrame.SizeChanged += RootFrame_SizeChanged;

            TitleBar.setTitleBar();

            Loaded += FramePage_Loaded;
        }

        private void RootFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 640)
            {
                MyPanel.ColumnCount = 1;
            }
            if (e.NewSize.Width >= 640)
            {
                MyPanel.ColumnCount = (int)(e.NewSize.Width / 400);
            }
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
