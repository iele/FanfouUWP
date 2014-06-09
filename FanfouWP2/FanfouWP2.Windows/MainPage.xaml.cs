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
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FanfouAPI.FanfouAPI.Instance.Login("elephas", "myelephas");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline();
        }
    }
}
