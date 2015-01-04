using Coding4Fun.Toolkit.Controls;
using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace FanfouWP2.Utils
{
    public static class ToastShow
    {
        public static void ShowToast(string title, string content)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(content));

            ToastNotification toast = new ToastNotification(toastXml);
            toast.SuppressPopup = false;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static void ShowInformation(string content, Action end)
        {
            ToastPrompt tp = new ToastPrompt();
            tp.Completed += (e, r) => end();
            tp.Title = "饭窗";
            tp.Message = content;
            tp.MillisecondsUntilHidden = 1000;
            tp.Show();
        }

        public static void ShowInformation(string content)
        {
            ShowInformation(content, () => { });
        }

    }
}

