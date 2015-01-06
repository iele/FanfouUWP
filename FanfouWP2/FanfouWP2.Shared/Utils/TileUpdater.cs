using FanfouWP2.CustomControl;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace FanfouWP2.Utils
{
    public static class TileUpdater
    {
        public static void Clear()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }

        public static void SetTile(string title, string msg, string[] imageUrls)
        {
            try
            {
                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150PeekImageCollection05);

                XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
                for (var i = 0; i < imageUrls.Length; i++)
                {
                    ((XmlElement)tileImageAttributes[i]).SetAttribute("src", imageUrls[i]);
                }

                XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
                tileTextAttributes[0].AppendChild(tileXml.CreateTextNode(title));
                tileTextAttributes[1].AppendChild(tileXml.CreateTextNode(msg));

                XmlDocument squareTileXml =
                    TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text02);

                XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
                squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode(title));
                squareTileTextAttributes[1].AppendChild(squareTileXml.CreateTextNode(msg));

                IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
                tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);


                var text = tileXml.GetXml();

                TileNotification tileNotification = new TileNotification(tileXml);
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

                System.Diagnostics.Debug.WriteLine(text);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}
