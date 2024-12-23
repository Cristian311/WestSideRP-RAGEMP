using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Shop
{
    internal class ShopHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        public ShopHandler()
        {
            Events.Add("SERVER:CLIENT:OpenShop", OpenShop);
            Events.Add("CEF:CLIENT:CloseShop", CloseShop);
            Events.Add("CEF:CLIENT:SaveItemData", SaveItemData);
        }

        private void OpenShop(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/shops/store.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");

            // Use a timeout to ensure the browser is fully loaded before executing JS
            cefBrowser.ExecuteJs($"UpdateItems(\"{escapedJson}\");");

        }

        private void SaveItemData(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:SaveItemData", json);
        }

        private void CloseShop(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:SaveItemData", json);

            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }
    }
}