using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character.Attributes
{
    internal class Attributes : Events.Script
    {
        private HtmlWindow cefBrowser;
        public Attributes()
        {
            Events.Add("SERVER:CLIENT:OpenAttributesScreen", OpenAttributesScreen);
            Events.Add("CEF:CLIENT:AttributesUpdate", AttributesUpdate);
        }

        private void AttributesUpdate(object[] args)
        {
            Events.CallRemote("CLIENT:SERVER:AttributesUpdate", args[0].ToString());
            cefBrowser.Destroy();
        }

        private void OpenAttributesScreen(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package/html/attributes/attributes.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            bool darkmode = bool.Parse(args[0].ToString());
            if(darkmode)
                cefBrowser.ExecuteJs("toggleDarkMode();");
        }
    }
}
