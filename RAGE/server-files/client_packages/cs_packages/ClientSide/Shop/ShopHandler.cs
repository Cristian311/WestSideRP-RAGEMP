using Newtonsoft.Json;
using RAGE;
using RAGE.Ui;

namespace ClientSide.Shop
{
    internal class ShopHandler : Events.Script
    {
        private static HtmlWindow cefBrowser;
        public ShopHandler()
        {
            Events.Add("SERVER:CLIENT:OpenShop", OpenShop);
            Events.Add("CEF:CLIENT:CloseShop", CloseShop);
            Events.Add("CEF:CLIENT:SaveItemData", SaveItemData);
            Events.Add("CEF:CLIENT:BuyShopItems", BuyShopItems);
        }

        private void BuyShopItems(object[] args)
        {
            var json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:BuyShopItems", json);

            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }

        private void OpenShop(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package/html/shops/store.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            escapedJson = JsonConvert.SerializeObject(json);

            bool adminBool = bool.Parse(args[1].ToString());
            //Chat.Output(adminBool.ToString());

            cefBrowser.ExecuteJs("setTimeout(() => UpdateItems(" + escapedJson + "), 100);");
            //cefBrowser.ExecuteJs("setTimeout(() => toggleAdmin(" + adminBool.ToString().ToLower() + "), 100);");
            cefBrowser.ExecuteJs("setTimeout(() => toggleAdmin(false), 100);");

        }

        private void SaveItemData(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:SaveItemData", json);
        }

        public static void CloseShop(object[] args)
        {
            //    string json = args[0].ToString();
            //    Events.CallRemote("CLIENT:SERVER:SaveItemData", json);

            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }
    }
}