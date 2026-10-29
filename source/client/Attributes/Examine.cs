using RAGE;
using RAGE.Ui;

namespace ClientSide
{
    public class Examine : Events.Script
    {
        private static HtmlWindow cefBrowser;
        public Examine()
        {
            Events.Add("SERVER:CLIENT:OpenExamineScreen", OpenExamineScreen);
        }

        public static void CloseExamineScreen()
        {
            if (cefBrowser != null)
            {
                cefBrowser.Destroy();
                cefBrowser = null;
            }
            //cefBrowser.ExecuteJs("toggleUI(false);");
        }
        private void OpenExamineScreen(object[] args)
        {
            if (cefBrowser != null)
            {
                cefBrowser.Destroy();
                cefBrowser = null;
            }
            cefBrowser = new HtmlWindow("http://package/html/attributes/examine.html");
            cefBrowser.Active = true;

            var Age = args[0].ToString();
            var Description = args[1].ToString();
            var Outfit = args[2].ToString();
            var Name = args[4].ToString();
            cefBrowser.ExecuteJs($"updateCharacterAge({Age});");
            cefBrowser.ExecuteJs($"updateCharacterDescription('{Name}', '{Description}');");
            cefBrowser.ExecuteJs($"updateOutfitDescription('{Outfit}');");

            bool tattoos = bool.Parse(args[3].ToString());
            cefBrowser.ExecuteJs($"toggleTattooCheck({tattoos});");

        }
    }
}
