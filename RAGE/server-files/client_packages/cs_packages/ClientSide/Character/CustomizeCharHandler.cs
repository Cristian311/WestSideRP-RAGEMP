using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character
{
    internal class CustomizeCharHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        public CustomizeCharHandler() 
        {
            Events.Add("CEF:CLIENT:showCustomizeCharScreen", ShowCustomizeCharScreen);
            Events.Add("SERVER:CLIENT:showCustomizeCharScreen", ShowCustomizeCharScreenServer);
            Events.Add("CEF:CLIENT:closeCustomizeCharScreen", CloseCustomizeCharScreen);
            Events.Add("CEF:CLIENT:updateCharacter", UpdateCharacter);
        }
        private void ShowCustomizeCharScreenServer(object[] args)
        {
            // Initialize the HTML window and show the cursor
            cefBrowser = new HtmlWindow("http://package2/html/Characters/customize_char.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            // Get the JSON string and output it to chat for debugging
            string json = args[0].ToString();
            RAGE.Chat.Output(json);

            // Escape the JSON for embedding in JavaScript
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");

            // Execute JavaScript and pass the escaped JSON string
            cefBrowser.ExecuteJs($"updateCharacterDataFromJson(\"{escapedJson}\");");
        }
        private void ShowCustomizeCharScreen(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/Characters/customize_char.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void CloseCustomizeCharScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;

            string DetailsJSON = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:ClosedCustomizeCharScreen", DetailsJSON); //send the json to server and and update the database
        }
        private void UpdateCharacter(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:CharCustomiseUpdate", json);
        }
    }
}
