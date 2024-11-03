
using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.DB;
using System.Collections.Generic;
using System;
using System.Text.Json;
using ServerSide.Interfaces;


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
            AccountDbManager accountDb = new AccountDbManager();
            if (accountDb.checkAccountLogin(username, password, out int? Id))
            {
                //LOG IN SUCCESFULLY
                NAPI.Util.ConsoleOutput(username + " s-a logat.");
                player.TriggerEvent("SERVER:CLIENT:closeAuthInterface");

                var account = new Account((int)Id, username, player);
                AccountManager.loggedAccounts.Add(account);
                AccountManager.AccountPlayerPairs.Add(account, player);

                //CHARACTER SELECT
                CharacterDbManager characterDb = new CharacterDbManager();
                List<ICharacter> characters = characterDb.getCharacters(account);
                

                CharacterManager cm = new CharacterManager();
                cm.getNamesAndMoneyForCEF(characters, cm.getRequiredCount(characters), out string[] names, out int[] money);
                string namesJSON = JsonSerializer.Serialize(names);
                string moneyJSON = JsonSerializer.Serialize(money);
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
