using RAGE;
using RAGE.Ui;

namespace ClientSide.Player.HUD
{
    public class LogoAndVersion : Events.Script
    {
        private static HtmlWindow cefBrowser = new HtmlWindow("http://package/hud/logo.html");
        private static string versionText;
        public LogoAndVersion()
        {
            Events.Add("SERVER:CLIENT:ShowLogo", ShowLogo);
        }
        public static void toggleUI(bool toggle)
        {
            if (toggle)
            {
                cefBrowser = new HtmlWindow("http://package/hud/logo.html");
                cefBrowser.ExecuteJs($"updateText(\"{versionText}\");");
            }
            else
                cefBrowser.Destroy();
        }
        private void ShowLogo(object[] args)
        {
            //cefBrowser = new HtmlWindow("http://package/hud/logo.html");
            string text = args[0].ToString();
            versionText = text;
            cefBrowser.ExecuteJs($"updateText(\"{text}\");");
        }
    }
}
