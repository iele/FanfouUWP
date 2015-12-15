using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using FanfouUWP.Utils;
using Windows.UI.Xaml.Input;
using FanfouUWP.FanfouAPI.Items;
using FanfouUWP.Common;
using Windows.UI.Core;
using Windows.Foundation.Metadata;

namespace FanfouUWP
{
    public sealed partial class MainPage : Page
    {

        private User currentUser { get; set; } = SettingStorage.Instance.currentUser;

        FanfouAPI.RestClient.LoadingState isLoading = FanfouAPI.RestClient.Loading;

        public MainPage()
        {
            InitializeComponent();

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, e) =>
                {
                    if (ScenarioFrame.CanGoBack)
                    {
                        e.Handled = true;
                        ScenarioFrame.GoBack();
                    }
                };
            }
            
            ToastShow.currentMainPage = this;

            isLoading.PropertyChanged += IsLoading_PropertyChanged;

            Loaded += MainPage_Loaded;
        }

        public async void showInformation(string msg)
        {
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ScenarioFrame.CanGoBack)
            {
                e.Handled = true;
                ScenarioFrame.GoBack();
            }
        }

        private void IsLoading_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            loading.IsActive = isLoading.isLoading;
        }

        private void FavoriteGrid_Tapped(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(FavoritePage), Utils.DataConverter<User>.Convert(currentUser));
        }

        private void FindGrid_Tapped(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(FindPage));
        }

        private void SearchGrid_Click(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(SearchPage));
        }

        private void TrendsGrid_Tapped(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(TrendsPage));
        }

        private void SelfGrid_Tapped(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(SelfPage), Utils.DataConverter<User>.Convert(currentUser));
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            MenuSplitter.IsPaneOpen = true;
        }

        private void DirectGrid_Tapped(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(DirectPage));
        }

        private void ScenarioFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(HomePage));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = (Splitter.IsPaneOpen == true) ? false : true;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(AboutPage));
        }
        private void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(InformationPage));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.ScenarioFrame == null)
                return;

            if (this.ScenarioFrame.CanGoBack)
                this.ScenarioFrame.GoBack();
        }

        private void MessageGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(HomePage));
        }

        private void ScenarioFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void SendGrid_Click(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(SendPage));
        }

        private void SelfGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(SelfPage));
        }

        private void menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Splitter.IsPaneOpen = false;
        }

        private void InfromationStoryBoard_Completed(object sender, object e)
        {
            InfromationDisappearStoryBoard.Begin();
        }

        private void InfromationDisappearStoryBoard_Completed(object sender, object e)
        {
            this.InformationStackPanel.Visibility = Visibility.Collapsed;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Utils.SettingStorage.Instance.currentUser = null;
            Utils.NavigationControl.ClearStack(App.RootFrame);
            App.RootFrame.Navigate(typeof(LoginPage));
        }
    }
}