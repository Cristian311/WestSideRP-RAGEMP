using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Player.HUD
{
    public class LogoAndVersion : Events.Script
    {
        private static HtmlWindow cefBrowser;
        public LogoAndVersion() 
        {
            Events.Add("SERVER:CLIENT:ShowLogo", ShowLogo);
        }

        private void ShowLogo(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package/hud/logo.html");
            string text = args[0].ToString();
            cefBrowser.ExecuteJs($"updateText({text});");
            cefBrowser.ExecuteJs($"setTimeout(() => updateText({text}), 100);");
        }
    }
}
