using RAGE;
using RAGE.Ui;

namespace ClientSide.Character
{
    internal class CreateCharHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        CreateCharHandler()
        {
            Events.Add("CEF:CLIENT:ShowCreateCharScreen", ShowCreateCharScreen);
            Events.Add("CEF:CLIENT:CloseCreateCharInterface", CloseCreateCharInterface);
            Events.Add("CEF:CLIENT:CreateNewCharacter", CreateNewCharacter);

        }
        private void CreateNewCharacter(object[] args)
        {
            string firstname = args[0].ToString();
            string lastname = args[1].ToString();
            string selectedGender = args[2].ToString();
            Events.CallRemote("CLIENT:SERVER:CreateCharacter", firstname, lastname, selectedGender);
        }

        private void ShowCreateCharScreen(object[] args)
        {
            //CloseCreateCharInterface(null);
            cefBrowser = new HtmlWindow("http://package/html/characters/create_character.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void CloseCreateCharInterface(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
            Events.CallRemote("CLIENT:SERVER:ClosedCreateCharScreen");
        }

    }
}