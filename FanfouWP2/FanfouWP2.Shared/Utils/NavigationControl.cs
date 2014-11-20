using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FanfouWP2.Utils
{
    public class NavigationControl
    {
        public static void ClearStack(Frame frame)
        {
            if (frame == null)
                frame = Window.Current.Content as Frame;

            if (frame != null)
                frame.SetNavigationState("1,0");
        }
    }
}