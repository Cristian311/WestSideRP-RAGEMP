
using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.DB;
using System.Collections.Generic;
using System;
using System.Text.Json;
using ServerSide.Interfaces;
using ServerSide.DB.CharacterDB;
using ServerSide.Util;
using ServerSide.Admin;


namespace ServerSide.AuthServer
{
    internal class LoginServer : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        private void onPlayerConnected(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:showAuthInterface");
        }
        [ServerEvent(Event.PlayerDisconnected)]
        private void onPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            Account? account = AccountManager.PlayerAccountPairs[player];
            if (account != null)
                AccountManager.idManager.ReclaimId(account.GameId);
                
            AccountManager.PlayerAccountPairs.Remove(player);
            AccountManager.AccountPlayerPairs.Remove(account);
            AccountManager.loggedAccounts.Remove(account);
        }
        [Command("login")]
        private void LoginCommand(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:showAuthInterface");
        }


        [RemoteEvent("CLIENT:SERVER:clientLogin")]
        public void OnLoginAttempt(Player player, string username, string password)
        {
            AccountDbManager accountDb = new AccountDbManager();
            if (accountDb.CheckAccountLogin(username, password, out int? Id))
            {
                //LOG IN SUCCESFULLY
                NAPI.Util.ConsoleOutput(username + " s-a logat.");
                player.TriggerEvent("SERVER:CLIENT:closeAuthInterface");
                
                var account = new Account((int)Id, username, player);
                if(accountDb.CheckAccountAdminLevel(username, out int? adminLevel))
                {
                    if (!AccountManager.loggedAccounts.Contains(account))
                        AccountManager.loggedAccounts.Add(account);
                    account.AdminLevel = adminLevel.GetValueOrDefault();
                    AdminManager.SendChatMessageToAdmins($"Admin {username} Level {adminLevel} s-a logat.");
                }
                account.GameId = AccountManager.idManager.AssignId();
                if(!AccountManager.loggedAccounts.Contains(account))
                    AccountManager.loggedAccounts.Add(account);
                if(!AccountManager.AccountPlayerPairs.ContainsKey(account))
                    AccountManager.AccountPlayerPairs.Add(account, player);
                if(!AccountManager.PlayerAccountPairs.ContainsKey(player))
                    AccountManager.PlayerAccountPairs.Add(player, account);

                //CHARACTER SELECT
                CharacterManager.GetNameMoneyJson(account, out string namesJSON, out string moneyJSON);
                player.TriggerEvent("SERVER:CLIENT:showCharInterface");
                player.TriggerEvent("SERVER:CLIENT:displayAccountInfo", username, namesJSON, moneyJSON); 
            }
            else
            {//login unsuccesfull
                player.TriggerEvent("SERVER:CLIENT:displayLoginFail");
            }
        }
    }
}
