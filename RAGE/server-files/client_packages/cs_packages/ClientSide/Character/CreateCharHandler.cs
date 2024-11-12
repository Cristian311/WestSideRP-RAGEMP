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
            Events.Add("CEF:CLIENT:closeCreateCharInterface", CloseCreateCharInterface);
            Events.Add("CEF:CLIENT:CreateNewCharacter", CreateNewCharacter);
            
        }
        private void CreateNewCharacter(object[] args) 
        {
            string firstname = args[0].ToString();
            string lastname = args[1].ToString();
            string selectedGender = args[2].ToString();
            RAGE.Chat.Output(firstname + selectedGender);
            Events.CallRemote("CLIENT:SERVER:CreateCharacter", firstname, lastname, selectedGender);
        }
        
        private void ShowCreateCharScreen(object[] args)
        {
            //CloseCreateCharInterface(null);
            cefBrowser = new HtmlWindow("http://package2/html/Characters/create_character.html"); 
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void CloseCreateCharInterface(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
            //RAGE.Chat.Output("rrrr");
            Events.CallRemote("CLIENT:SERVER:ClosedCreateCharScreen");
        }
        
    }
}