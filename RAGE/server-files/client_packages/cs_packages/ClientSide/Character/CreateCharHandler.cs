using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace ClientSide.Character
{
    internal class CreateCharHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        CreateCharHandler()
        {
            Events.Add("CEF:CLIENT:showCreateCharScreen", ShowCreateCharScreen);
            Events.Add("CEF:CLIENT:showCustomizeCharScreen", ShowCustomizeCharScreen);
            Events.Add("CEF:CLIENT:CreateNewCharacter", CreateNewCharacter);
            Events.Add("CEF:CLIENT:closeCustomizeCharScreen", closeCustomizeCharScreen);
            Events.Add("CEF:CLIENT:closeCreateCharInterface", CloseCreateCharInterface);
            Events.Add("CEF:CLIENT:updateCharacter", UpdateCharacter);
        }
        private void UpdateCharacter(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:CharCustomiseUpdate", json);
        }
        private void CreateNewCharacter(object[] args) 
        {
            string firstname = args[0].ToString();
            string lastname = args[1].ToString();
            string selectedGender = args[2].ToString();
            Events.CallRemote("CLIENT:SERVER:CreateCharacter", firstname, lastname, selectedGender);
        }
        private void ShowCustomizeCharScreen(object[] args)
        {
            //CloseCreateCharInterface(null);
            cefBrowser = new HtmlWindow("C:\\Users\\sarof\\Desktop\\RAGE MP SERVERS\\GM DE RP .NET CORE 3.1\\RAGE\\server-files\\client_packages\\cs_packages\\ClientSide\\html\\Characters\\customize_char.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

        }
        private void ShowCreateCharScreen(object[] args)
        {
            //CloseCreateCharInterface(null);
            cefBrowser = new HtmlWindow("C:\\Users\\sarof\\Desktop\\RAGE MP SERVERS\\GM DE RP .NET CORE 3.1\\RAGE\\server-files\\client_packages\\cs_packages\\ClientSide\\html\\Characters\\create_character.html"); 
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void CloseCreateCharInterface(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }
        private void closeCustomizeCharScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }
    }
}