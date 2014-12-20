//using NotificationsExtensions.ToastContent;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
namespace FanfouWP2.Utils
{
    public class ToastShow
    {
        public static void ShowToast(string title, string content)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(content));

            ToastNotification toast = new ToastNotification(toastXml);
            toast.SuppressPopup = true;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}