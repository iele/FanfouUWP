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
            if (SettingStorage.Instance.currentUser== null)
            {
                NavigationControl.ClearStack(Frame);
                Frame.Navigate(typeof (LoginPage));
            }
            else
            {
                FanfouAPI.FanfouAPI.Instance.setUser(SettingStorage.Instance.currentUser);
                NavigationControl.ClearStack(Frame);
                Frame.Navigate(typeof(HomePage));
            }
        }
    }
}