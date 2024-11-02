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
        }
        private void ShowCharInterface(object[] args)
        {
            cefBrowser = new HtmlWindow("C:\\Users\\sarof\\Desktop\\RAGE MP SERVERS\\GM DE RP .NET CORE 3.1\\RAGE\\server-files\\client_packages\\cs_packages\\ClientSide\\html\\Characters\\characters.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
            RAGE.Chat.Output("x");
        }
        private void CloseCharInterface(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }

        
        private void DisplayAccountInfo(object[] args)
        {
                RAGE.Chat.Output("aci");
            RAGE.Chat.Output($"Number of args: {args.Length}");
            RAGE.Chat.Output($"Arg 0 Type: {args[0].GetType()}");
            RAGE.Chat.Output($"Arg 1 Type: {args[1].GetType()}");
            RAGE.Chat.Output($"Arg 2 Type: {args[2].GetType()}");
            if (args.Length < 3)
            {
                RAGE.Chat.Output("Error: Insufficient arguments passed to displayAccountInfo.");
                return;
            }
            //string accountName = args[0].ToString();
            //string[] charNames = (string[])args[1];
            //int[] money = (int[])args[2];
            //    RAGE.Chat.Output("aci2");
            //string charNamesJs = string.Join(",", charNames.Select(name => $"\"{name}\""));
            //string moneyJs = string.Join(",", money);

            //RAGE.Chat.Output($"Account Name: {accountName}");
            //RAGE.Chat.Output($"Character Names: [{charNamesJs}]");
            //RAGE.Chat.Output($"Money: [{moneyJs}]");
            //    RAGE.Chat.Output("aci3");
            //cefBrowser.ExecuteJs($"displayAccountInfo(`{accountName}`, [{charNamesJs}], [{moneyJs}]);");
            //    RAGE.Chat.Output("aci4");
        }
    }
}
