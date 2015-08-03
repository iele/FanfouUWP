using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace FanfouUWP.ItemControl
{
    public sealed partial class DirectItemControl : UserControl
    {
        public DirectItemControl()
        {
            InitializeComponent();
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        private void RectangleImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }
    }
}