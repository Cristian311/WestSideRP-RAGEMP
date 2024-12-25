using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character.Inventory
{
    internal class InventoryHandler : Events.Script
    {
        private static HtmlWindow cefBrowser;
        public InventoryHandler() 
        {
            Events.Add("SERVER:CLIENT:OpenInventory", OpenInventory);
            Events.Add("CEF:CLIENT:CloseInventory", CloseInventory);
        }

        public static void CloseInventory(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }

        private void OpenInventory(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/inventory/inventory.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
    }
}
