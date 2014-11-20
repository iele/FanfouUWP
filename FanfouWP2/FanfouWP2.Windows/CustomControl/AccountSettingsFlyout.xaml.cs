using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FanfouWP2.CustomControl
{
    public sealed partial class AccountSettingsFlyout : SettingsFlyout
    {
        public AccountSettingsFlyout()
        {
            InitializeComponent();

            if (FanfouAPI.FanfouAPI.Instance.currentUser != null)
                DataContext = FanfouAPI.FanfouAPI.Instance.currentUser;
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
            rootFrame.Navigate(typeof (LoginPage));
        }
    }
}