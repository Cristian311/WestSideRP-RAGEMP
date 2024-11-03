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
            Events.Add("SERVER:CLIENT:displayAccountInfo", DisplayAccountInfo);
            Events.Add("CEF:CLIENT:characterSelected", CharacterSelected);
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

            // Split the strings into arrays
            //var names = RAGE.Util.Json.Deserialize<string[]>(namesString);
            //var money = RAGE.Util.Json.Deserialize<int[]>(moneyString);

            //// Join the names and money arrays into a single string formatted for JavaScript
            //string namesJsArray = string.Join(",", names.Select(name => $"\"{name.Trim()}\"")); // Quoting each name
            //string moneyJsArray = string.Join(",", money); // No quotes for numbers

            // Execute the JavaScript function with the username and arrays
            cefBrowser.ExecuteJs($"displayAccountInfo(`{username}`, {namesString}, {moneyString});");

        }
        private void CharacterSelected(object[] args)
        {
            string CharacterName = args[0].ToString();
            RAGE.Chat.Output(CharacterName);
        }
    }
}
