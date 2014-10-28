using System;
using System.Collections.Generic;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
//using NotificationsExtensions.ToastContent;

namespace FanfouWP2.Utils
{
    public class ToastShow
    {
        public static void ShowToast(string title, string content)
        {
            /*
            IToastText02 templateContent = ToastContentFactory.CreateToastText02();
            templateContent.TextHeading.Text = title;
            templateContent.TextBodyWrap.Text = content;
            templateContent.Duration = ToastDuration.Short;

            ToastNotification toast = new ToastNotification(templateContent.GetXml());            
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            */
        }

    }
}
