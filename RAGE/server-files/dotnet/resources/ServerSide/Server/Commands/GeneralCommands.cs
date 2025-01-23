using GTANetworkAPI;
using Org.BouncyCastle.Asn1.X509;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server.Commands
{
    internal class GeneralCommands : Script
    {
        [Command("stats")]
        private void stats_cmd(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            var character = CharacterManager.GetCharacter(player);
            if (character == null) return;

            var aduty = account.AdminDuty ? "Yes" : "No";
            var faction = FactionManager.GetFactionById(character.Id);
            var factionName = faction != null ? faction.Name : "None";
            player.SendChatMessage($"{Colors.DARK_YELLOW}-----------------------");
            player.SendChatMessage($"{Colors.DARK_YELLOW}Nume cont: {Colors.WHITE}{account.Name} #{account.Id}{Colors.DARK_YELLOW}, Admin level: {Colors.WHITE}{account.AdminLevel}{Colors.DARK_YELLOW}, Admin duty: {Colors.WHITE}{aduty}");
            player.SendChatMessage($"{Colors.DARK_YELLOW}Nume caracter: {Colors.WHITE}{character.Name} #{character.Id}, {Colors.DARK_YELLOW}ID: {Colors.WHITE}{character.GameId}{Colors.DARK_YELLOW}, Cash: {Colors.GREEN}{character.Money}${Colors.DARK_YELLOW}, Banca: {Colors.GREEN}{character.BankMoney}$");
            player.SendChatMessage($"{Colors.DARK_YELLOW}Current outfit: {Colors.WHITE}{character.CurrentOutfit}{Colors.DARK_YELLOW}, Varsta: {Colors.WHITE}{character.Age}{Colors.DARK_YELLOW}, Factiune: {Colors.WHITE}{factionName}{Colors.DARK_YELLOW}, Rank: {Colors.WHITE}{character.factionRank}");
            player.SendChatMessage($"{Colors.DARK_YELLOW}-----------------------");
        }
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
