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
            Events.Add("SERVER:CLIENT:ShowCharInterface", ShowCharInterface);
            Events.Add("CEF:CLIENT:ShowCharInterface", ShowCharInterface);

            Events.Add("SERVER:CLIENT:CloseCharInterface", CloseCharInterface);
            Events.Add("CEF:CLIENT:CloseCharInterface", CloseCharInterface);

            Events.Add("SERVER:CLIENT:DisplayAccountInfo", DisplayAccountInfo);
            Events.Add("CEF:CLIENT:CharacterSelected", CharacterSelected);
        }
        
        private void ShowCharInterface(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/Characters/characters.html");
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

            Events.CallRemote("CLIENT:SERVER:CharSelected", CharacterName);
            Events.CallRemote("CLIENT:SERVER:CheckIfCharExistsForCustomizeScreen");
        }
    }
}
