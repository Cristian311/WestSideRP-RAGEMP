using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Util;

namespace ServerSide.Server.Commands
{
    public class GeneralCommands : Script
    {
        [Command("stats")]
        private static void stats_cmd(Player player)
        {
            var account = AccountManager.GetAccount(player);
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
        private static void cmd_id(Player player, string target)
        {
            var character = CharacterManager.GetCharacterByNameOrId(target);
            if (character == null) return;

            player.SendChatMessage($"ID [{character.GameId}] {character.Name}");
        }
    }
}
