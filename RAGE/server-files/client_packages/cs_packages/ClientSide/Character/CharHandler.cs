using RAGE.Ui;
using RAGE;
using System.Linq;
using RAGE.Util;
namespace ClientSide.Character
{
    public class CharHandler : Events.Script
    {
        private HtmlWindow cefBrowser;

        public CharHandler()
        {
            Events.Add("SERVER:CLIENT:showCharInterface", ShowCharInterface);
            Events.Add("SERVER:CLIENT:closeCharInterface", CloseCharInterface);
            Events.Add("CEF:CLIENT:closeCharInterface", CloseCharInterface);
            Events.Add("SERVER:CLIENT:displayAccountInfo", DisplayAccountInfo);
            Events.Add("CEF:CLIENT:characterSelected", CharacterSelected);
            Events.Add("CEF:CLIENT:showCreateCharScreen", ShowCreateCharScreen);
        }
        private void ShowCreateCharScreen(object[] args)
        {
            CloseCharInterface(null);
            cefBrowser = new HtmlWindow("C:\\Users\\sarof\\Desktop\\RAGE MP SERVERS\\GM DE RP .NET CORE 3.1\\RAGE\\server-files\\client_packages\\cs_packages\\ClientSide\\html\\Characters\\create_character.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void ShowCharInterface(object[] args)
        {
            cefBrowser = new HtmlWindow("C:\\Users\\sarof\\Desktop\\RAGE MP SERVERS\\GM DE RP .NET CORE 3.1\\RAGE\\server-files\\client_packages\\cs_packages\\ClientSide\\html\\Characters\\characters.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void CloseCharInterface(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }

        
        private void DisplayAccountInfo(object[] args)
        {

            string username = args[0].ToString();
            string namesString = args[1].ToString();
            string moneyString = args[2].ToString();

            cefBrowser.ExecuteJs($"displayAccountInfo(`{username}`, {namesString}, {moneyString});");

        }
        private void CharacterSelected(object[] args)
        {
            string CharacterName = args[0].ToString();
            RAGE.Chat.Output(CharacterName);
            
        }
    }
}
