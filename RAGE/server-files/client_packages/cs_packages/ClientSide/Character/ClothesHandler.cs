using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character
{
    internal class ClothesHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        private ClothesHandler()
        {
            Events.Add("SERVER:CLIENT:openClothesScreen", OpenClothesScreen);
            Events.Add("SERVER:CLIENT:closeClothesScreen", CloseClothesScreen);
            Events.Add("CEF:CLIENT:closeClothesScreen", CloseClothesScreen);
            Events.Add("CEF:CLIENT:updateClothes", UpdateClothes);
        }
        private void UpdateClothes(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:ClothesUpdate", json);
        }
        private void CloseClothesScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }
        private void OpenClothesScreen(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/clothes/clothes.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");

            cefBrowser.ExecuteJs($"updateHairDataFromJson(\"{escapedJson}\");");
        }
    }
}
