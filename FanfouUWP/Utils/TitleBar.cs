using FanfouUWP.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace FanfouUWP.Utils
{
    public static class TitleBar
    {
        public static void setTitleBar()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            var titleBar = applicationView.TitleBar;

            titleBar.BackgroundColor = Colors.DeepSkyBlue;
            titleBar.ForegroundColor = Colors.White;

            titleBar.InactiveBackgroundColor = Colors.DeepSkyBlue;
            titleBar.InactiveForegroundColor = Colors.White;

            titleBar.ButtonInactiveBackgroundColor = Colors.DeepSkyBlue;
            titleBar.ButtonInactiveForegroundColor = Colors.White;

            titleBar.ButtonPressedBackgroundColor = Colors.SkyBlue;
            titleBar.ButtonPressedForegroundColor = Colors.White;

            titleBar.ButtonBackgroundColor = Colors.DeepSkyBlue;
            titleBar.ButtonForegroundColor = Colors.White;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }
    }
}
