using GTANetworkAPI;
using Org.BouncyCastle.Asn1.X509;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Util;
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
                        player.SendChatMessage($"ID [{id2}] {account.Name}: Playing as {characterName} Ping: {account.player.Ping}");
                        found = true;
                    }
                }
            }
            else
            {
                var chars = CharacterManager.getCharByName(id);
                if (chars.Count == 1)
                {
                    var character = chars[0];
                    if (!AdminManager.CheckIfAdminAccess(player, 1))
                        return;
                    var AdminAccount = AccountManager.PlayerAccountPairs[player];
                    var TargetAccount = CharacterManager.CharacterAccountPair[character];
                    found = true;
                    player.SendChatMessage($"ID [{id2}] {TargetAccount.Name}: Playing as {character.Name} Ping: {TargetAccount.player.Ping}");
                }
                else
                {
                    foreach (var character in chars)
                    {
                        player.SendChatMessage(character.Name);
                    }
                }
            }
            if (found == false)
                player.SendChatMessage("Couldn't find the player you're looking for");
        }
    }
}
