
using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Admin;
using ServerSide.Server.DB;
using ServerSide.Server.DB.CharacterDB;
using System.Linq;


namespace ServerSide.Server.AuthServer
{
    internal class LoginServer : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        private void onPlayerConnected(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:ShowAuthInterface");

        }
        [ServerEvent(Event.PlayerDisconnected)]
        private void onPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            if (AccountManager.PlayerAccountPairs.ContainsKey(player) == false)
                return;
            Account account = AccountManager.PlayerAccountPairs[player];
            if (account != null)
                AccountManager.idManager.ReclaimId(account.GameId);

            AccountManager.PlayerAccountPairs.Remove(player);
            AccountManager.AccountPlayerPairs.Remove(account);
            AccountManager.loggedAccounts.Remove(account);
        }
        [Command("login")]
        private void LoginCommand(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:ShowAuthInterface");
        }


        [RemoteEvent("CLIENT:SERVER:ClientLogin")]
        public void OnLoginAttempt(Player player, string username, string password)
        {
            AccountDbManager accountDb = new AccountDbManager();
            if (accountDb.CheckAccountLogin(username, password, out int? Id))
            {
                //LOG IN SUCCESFULLY
                NAPI.Util.ConsoleOutput(username + " s-a logat.");
                player.TriggerEvent("SERVER:CLIENT:CloseAuthInterface");

                var account = new Account((int)Id, username, player);
                if (accountDb.CheckAccountAdminLevel(username, out int? adminLevel))
                {
                    if (!AccountManager.loggedAccounts.Contains(account))
                        AccountManager.loggedAccounts.Add(account);
                    account.AdminLevel = adminLevel.GetValueOrDefault();
                    AdminManager.SendChatMessageToAdmins($"Admin {username} Level {adminLevel} s-a logat.");
                }

                account.GameId = AccountManager.idManager.AssignNewId(account);
                if (!AccountManager.loggedAccounts.Contains(account))
                    AccountManager.loggedAccounts.Add(account);
                if (!AccountManager.AccountPlayerPairs.ContainsKey(account))
                    AccountManager.AccountPlayerPairs.Add(account, player);
                if (!AccountManager.PlayerAccountPairs.ContainsKey(player))
                    AccountManager.PlayerAccountPairs.Add(player, account);

                //CHARACTER SELECT
                CharacterManager.ShowChangeCharacterScreen(account);

                foreach (var acc in AccountManager.loggedAccounts)
                {
                    acc.player.TriggerEvent("SERVER:CLIENT:UpdateDiscord", $"{AccountManager.loggedAccounts.Count}/1000");
                }
            }
            else
            {//login unsuccesfull
                player.TriggerEvent("SERVER:CLIENT:DisplayLoginFail");
            }
        }
    }
}
