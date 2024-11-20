using RAGE;
using RAGE.NUI;
using RAGE.Ui;
using RAGE.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character
{
    internal class TattooHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        private TattooHandler()
        {
            Events.Add("SERVER:CLIENT:showTattooScreen", showTattooScreen);
            Events.Add("CEF:CLIENT:stopTattooScreen", stopTattooScreen);
            Events.Add("CEF:CLIENT:ApplyTattoo", updateTattoo);

        }

        private void updateTattoo(object[] args)
        {
            string tattooName = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:ApplyTattoo", tattooName);
        //    string json = args[0].ToString();
        //    Events.CallRemote("CLIENT:SERVER:TattooCustomizeUpdate", json);
        }
        private void showTattooScreen(object[] args)
        {
            // Initialize the HTML window and show the cursor
            string json = args[0].ToString();
            cefBrowser = new HtmlWindow("http://package2/html/tattoos/tattoos.html");
            
            cefBrowser.Active = true;
            Cursor.Visible = true;
            Chat.Show(false); 
            //string json = args[0].ToString();
            //RAGE.Chat.Output(json);
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            cefBrowser.ExecuteJs($"updateDataFromJson(\"{escapedJson}\");");
            //
            //
        }

        private void stopTattooScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }
    }
}
