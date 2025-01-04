using RAGE;
using RAGE.Ui;

namespace ClientSide.Auth
{
    public class AuthHandle : Events.Script
    {
        private HtmlWindow cefBrowser;

        public AuthHandle()
        { 
            Events.Add("SERVER:CLIENT:ShowAuthInterface", ShowAuthInterface);
            Events.Add("SERVER:CLIENT:CloseAuthInterface", CloseAuthInterface);
            Events.Add("CEF:CLIENT:ClientLogin", onClientLogin);
            Events.Add("CEF:CLIENT:ClientRegister", onClientRegister);
            Events.Add("SERVER:CLIENT:DisplayLoginFail", displayLoginFail);
            Events.Add("SERVER:CLIENT:DisplayRegisterMessage", displayRegisterMessage);
        }
         
        private void ShowAuthInterface(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/loginregister/login_register.html");
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
            Events.CallRemote("CLIENT:SERVER:ClientLogin", username, password);
        }

        private void onClientRegister(object[] args)
        {
            string username = args[0].ToString();
            string password = args[1].ToString();
            Events.CallRemote("CLIENT:SERVER:ClientRegister", username, password);

        }
        private void displayLoginFail(object[] args)
        {
            cefBrowser.ExecuteJs($"displayLoginFail();");
        }
        private void displayRegisterMessage(object[] args)
        {
            string message = args[0].ToString();
            cefBrowser.ExecuteJs($"displayRegisterMessage(`{message}`);");
        }
    }
}
