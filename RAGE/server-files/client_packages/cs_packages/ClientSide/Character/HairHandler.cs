using RAGE;
using RAGE.NUI;
using RAGE.Ui;
using RAGE.Util;
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
            Events.Add("SERVER:CLIENT:ShowHairScreen", showHairScreen);
            Events.Add("CEF:CLIENT:StopHairScreen", stopHairScreen);
            Events.Add("CEF:CLIENT:UpdateHair", UpdateHair);
            
        }
        
        private void UpdateHair(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:HairCustomizeUpdate", json);
        }
        private void showHairScreen(object[] args)
        {
            // Initialize the HTML window and show the cursor
            cefBrowser = new HtmlWindow("http://package2/html/Characters/customize_hair.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            string json = args[0].ToString();
            RAGE.Chat.Output(json);

            // Escape the JSON for embedding in JavaScript
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            cefBrowser.ExecuteJs($"updateHairDataFromJson(\"{escapedJson}\");");
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
