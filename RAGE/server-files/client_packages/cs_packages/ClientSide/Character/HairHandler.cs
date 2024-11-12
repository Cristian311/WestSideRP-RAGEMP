using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character
{
    internal class HairHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        private HairHandler() 
        {
            Events.Add("SERVER:CLIENT:showHairScreen", showHairScreen);
            Events.Add("CEF:CLIENT:stopHairScreen", stopHairScreen);
            Events.Add("CEF:CLIENT:updateHair", UpdateHair);
        }
        private void UpdateHair(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:HairCustomizeUpdate", json);
        }
        private void showHairScreen(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/Characters/customize_hair.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void stopHairScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
            RAGE.Chat.Output("closed");

            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:UpdateHair", json);
        }
    }
}
