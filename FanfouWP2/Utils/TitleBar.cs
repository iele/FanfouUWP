using FanfouWP2.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace FanfouWP2.Utils
{
    public static class TitleBar
    {
        public static void setTitleBar()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            var titleBar = applicationView.TitleBar;

            titleBar.BackgroundColor = Colors.DeepSkyBlue;
            titleBar.ForegroundColor = Colors.White;

            titleBar.ButtonInactiveBackgroundColor = Colors.DeepSkyBlue;
            titleBar.ButtonInactiveForegroundColor = Colors.White;

            titleBar.ButtonPressedBackgroundColor = Colors.DeepSkyBlue;
            titleBar.ButtonPressedForegroundColor = Colors.White;

            titleBar.ButtonBackgroundColor = Colors.DeepSkyBlue;
            titleBar.ButtonForegroundColor = Colors.White;

            //CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            //Window.Current.SetTitleBar(new TitleBarControl());
        }
    }
}
