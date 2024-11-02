using Google.Protobuf.WellKnownTypes;
using GTANetworkAPI;
using ServerSide.Account;
using ServerSide.DB;
using System.Security.Cryptography.X509Certificates;

namespace ServerSide.AuthServer
{
    internal class LoginServer : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        private void onPlayerConnected(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:showAuthInterface");
        }

        [RemoteEvent("CLIENT:SERVER:clientLogin")]
        public void OnLoginAttempt(Player player, string username, string password)
        {
            AccountDbManager db = new AccountDbManager();
            if (db.checkAccountLogin(username, password))
            {
                //LOG IN SUCCESFULLY
                NAPI.Util.ConsoleOutput(username + " s-a logat.");
                player.TriggerEvent("SERVER:CLIENT:displayLoginResult", true);
                player.TriggerEvent("SERVER:CLIENT:closeAuthInterface");
                var account = new Account.Account(username, player);
                AccountManager.loggedAccounts.Add(account);
                AccountManager.AccountPlayerPairs.Add(account, player);
                
                //CHARACTER SELECT
                string[] names = new string[] { "Darius Campbell", "Andreea Gonzalez", "Michael Black" };
                int[] money = new int[] { 100, 500, 200 };
                player.TriggerEvent("SERVER:CLIENT:showCharInterface");
                player.TriggerEvent("SERVER:CLIENT:displayAccountInfo", username, names, money);
            }
            else
            {
                player.TriggerEvent("SERVER:CLIENT:displayLoginFail");
            }
        }
    }
}
