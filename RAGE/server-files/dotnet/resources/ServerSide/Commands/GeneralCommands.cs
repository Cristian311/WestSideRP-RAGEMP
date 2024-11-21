using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Commands
{
    internal class GeneralCommands : Script
    {
        [Command("id")]
        private void cmd_id(Player player, string id)
        {
            bool found = false;
            if (int.TryParse(id, out int id2))
            {
                foreach (var account in AccountManager.loggedAccounts)
                {
                    var characterName = CharacterManager.PlayerCharacterPair[account.player].Name;
                    if (account.GameId == id2)
                    {
                        player.SendChatMessage($"ID [{id2}] {account.Name}: Playing as {characterName}");
                        found = true;
                    }
                }
            }
            if (found == false)
                player.SendChatMessage("Couldn't find the id you're looking for");
        }
    }
}
