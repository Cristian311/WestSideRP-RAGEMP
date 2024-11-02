using RAGE;
using RAGE.Ui;

namespace ClientSide.Auth
{
    public class AuthHandle : Events.Script
    {
        private HtmlWindow cefBrowser;

        public AuthHandle()
        {
            Events.Add("SERVER:CLIENT:showAuthInterface", ShowAuthInterface);
            Events.Add("SERVER:CLIENT:closeAuthInterface", CloseAuthInterface);
            Events.Add("CEF:CLIENT:clientLogin", onClientLogin);
            Events.Add("CEF:CLIENT:clientRegister", onClientRegister);
            Events.Add("SERVER:CLIENT:displayLoginFail", displayLoginFail);
            Events.Add("SERVER:CLIENT:displayRegisterFail", displayRegisterFail);
        }
        private void ShowAuthInterface(object[] args)
        {
            cefBrowser = new HtmlWindow("C:\\Users\\sarof\\Desktop\\RAGE MP SERVERS\\GM DE RP .NET CORE 3.1\\RAGE\\server-files\\client_packages\\cs_packages\\ClientSide\\html\\login_register.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void CloseAuthInterface(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }
        private void onClientLogin(object[] args)
        {
            string username = args[0].ToString();
            string password = args[1].ToString();
            Events.CallRemote("CLIENT:SERVER:clientLogin", username, password);
            RAGE.Chat.Output(username + password);
        }

        private void onClientRegister(object[] args)
        {
            string username = args[0].ToString();
            string password = args[1].ToString();
            Events.CallRemote("CLIENT:SERVER:clientRegister", username, password);

        }
        private void displayLoginFail(object[] args)
        {
            cefBrowser.ExecuteJs($"displayLoginFail();");
        }
        private void displayRegisterFail(object[] args)
        {
            cefBrowser.ExecuteJs($"displayRegisterFail();");
        }
    }
}
