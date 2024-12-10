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
        }

        private void OpenShop(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/shops/store.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void CloseShop(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
            //Events.CallRemote("CLIENT:SERVER:ClosedCreateCharScreen");
        }
    }
}
