using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Player.HUD
{
    public class LogoAndVersion : Events.Script
    {
        private HtmlWindow cefBrowser;
        public LogoAndVersion() 
        {
            Events.Add("SERVER:CLIENT:ShowLogo", ShowLogo);
        }

        private void ShowLogo(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package/hud/logo.html");
            cefBrowser.ExecuteJs($"updateText({args[0]});");
            cefBrowser.ExecuteJs($"setTimeout(() => updateText({args[0]}), 100);");
        }
    }
}
