using GTANetworkAPI;
using ServerSide.Account;

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
            DatabaseHelper db = new DatabaseHelper();
            if (db.checkAccountLogin(username, password))
            {
                NAPI.Util.ConsoleOutput(username + " s-a logat.");
                player.TriggerEvent("SERVER:CLIENT:displayLoginResult", true);
                player.TriggerEvent("SERVER:CLIENT:closeAuthInterface");
                var account = new Account.Account(username, player);
                AccountManager.loggedAccounts.Add(account);
                AccountManager.AccountPlayerPairs.Add(account, player);
            }
            else
            {
                player.TriggerEvent("SERVER:CLIENT:displayLoginFail");
            }
        }
    }
}
