using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.Logger;
using ServerSide.Util;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Server.Commands
{
    internal class AdminCommands : Script
    {
        AccountManager accountManager = new AccountManager();

        [RequiresAdmin]
        [Command("dimension", "/dimension [id] | Dimensiunea trebuie sa fie un numar pozitiv, 0 pentru dimensiunea globala.")]
        private void cmd_dimension(Player player, uint dimension)
        {
            player.Dimension = dimension;
            player.SendChatMessage($"{Colors.INFO}Ti-ai setat dimensiunea ca: " + player.Dimension);
        }

        [RequiresAdmin(5)]
        [Command("makeadmin")]
        private void cmd_makeadmin(Player player, string target, int adminLevel = 1)
        {
            var account = AccountManager.GetAccount(player);
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var targetAccount = AccountManager.GetAccount(targetCharacter.player);
            targetAccount.AdminLevel = adminLevel;

            AdminManager.SendChatMessageToAdmins($"Adminul {account.AdminLevel} {account.Name} l-a facut admin {adminLevel} pe {targetAccount.Name}");
            
        }
        [RequiresAdmin]
        [Command("createveh")]
        public void cmd_createveh(Player player, string car, int color1 = 0, int color2 = 0)
        {
            VehicleHash busModel = NAPI.Util.VehicleNameToModel(car); //Gets the hash of bus model
            NAPI.Vehicle.CreateVehicle(busModel, player.Position, 0, color1, color2); //Creates a bus on the player's position
        }
        [RequiresAdmin]
        [Command("die")]
        public void cmd_die(Player player)
        {
            player.Kill();
        }
        [RequiresAdmin]
        [Command("gotoxyz")]
        public void cmd_gotoxyz(Player player, float x, float y, float z)
        {
            Vector3 position = new Vector3(x, y, z);
            player.Position = position;
        }
        [RequiresAdmin]
        [Command("weapon")]
        public void cmd_weapon(Player player, string weapon, int ammo)
        {
            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(weapon);
            player.GiveWeapon(weaponHash, ammo);
        }

        [RequiresAdmin]
        [Command("weather")]
        public void cmd_weather(Player player, string weather)
        {

            NAPI.World.SetWeather(weather);
            player.SendNotification($"Set weather to {weather}.");
        }

        [RequiresAdmin]
        [Command("time")]
        public void cmd_time(Player player, int hour)
        {
            NAPI.World.SetTime(hour, 0, 0);
            player.SendNotification($"Set time to {hour}.");
        }

        // MODERATION COMMANDS
        [RequiresAdmin]
        [Command("kick", GreedyArg = true)]
        public void cmd_kick(Player player, string target, string reason)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var account = AccountManager.GetAccount(player);
            var targetAccount = AccountManager.GetAccount(targetCharacter.player);

            targetAccount.player.Kick($"You were kicked by an admin. Reason: {reason}");

            string message = $"{Colors.ERROR}Admin {account.Name} i-a dat kick lui {targetAccount.currentCharacter.Name} Motiv: {reason}";
            NAPI.Chat.SendChatMessageToAll(message);

            ServerLogger.LogWithAccount(player, message);
        }

        [RequiresAdmin]
        [Command("ban")]
        public void cmd_ban(Player player, string target, string reason)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var account = AccountManager.GetAccount(player);
            var targetAccount = AccountManager.GetAccount(targetCharacter.player);

            targetAccount.player.Ban($"You were banned by an admin. Reason: {reason}");
            NAPI.Chat.SendChatMessageToAll($"{Colors.ERROR}Admin {account.Name} i-a dat ban lui {targetAccount.currentCharacter.Name} Motiv: {reason}");
        }

        [RequiresAdmin]
        [Command("mute")]
        public void cmd_mute(Player player, string target, string reason)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var account = AccountManager.GetAccount(player);
            var targetAccount = AccountManager.GetAccount(targetCharacter.player);

            targetAccount.Muted = true;
            NAPI.Chat.SendChatMessageToAll($"{Colors.ERROR}Admin {account.Name} i-a dat mute lui {targetAccount.currentCharacter.Name} Motiv: {reason}");
        }

        [RequiresAdmin]
        [Command("unmute")]
        public void cmd_unmute(Player player, string target)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var adminAccount = AccountManager.GetAccount(player);
            var targetAccount = AccountManager.GetAccount(targetCharacter.player);

            targetAccount.Muted = false;
            NAPI.Chat.SendChatMessageToAll($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat unmute lui {targetAccount.currentCharacter.Name}");
        }

        // TELEPORT COMMANDS
        [RequiresAdmin]
        [Command("goto")]
        public void cmd_goto(Player player, string target)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var adminAccount = AccountManager.GetAccount(player);

            player.Position = targetCharacter.Account.player.Position;

            AdminManager.SendChatMessageToAdmins($"{Colors.ERROR}Admin {adminAccount.Name} s-a teleportat la {targetCharacter.Name}");
        }

        [RequiresAdmin]
        [Command("bring")]
        public void cmd_bring(Player player, string target)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var adminAccount = AccountManager.GetAccount(player);
            targetCharacter.player.Position = player.Position;

            AdminManager.SendChatMessageToAdmins($"{Colors.ERROR}Admin {adminAccount.Name} l-a teleportat pe {targetCharacter.Name} la el");
        }

        // FREEZE/UNFREEZE COMMANDS

        [RequiresAdmin]
        [Command("aduty")]
        private void cmd_aduty(Player player)
        {
            var account = AccountManager.GetAccount(player);
            if (account.AdminDuty == false)
            {
                account.AdminDuty = true;
                AdminManager.SendChatMessageToAdmins($"Adminul {account.AdminLevel} {account.Name} este acum on-duty.");
            }
            else
            {
                account.AdminDuty = false;
                AdminManager.SendChatMessageToAdmins($"Adminul {account.AdminLevel} {account.Name} este acum off-duty.");
            }
        }


        [RequiresAdmin]
        [Command("freeze")]
        public void cmd_freeze(Player player, string target)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var adminAccount = AccountManager.GetAccount(player);
            targetCharacter.player.TriggerEvent("SERVER:CLIENT:FreezePlayer");

            AdminManager.SendChatMessageToAdmins($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat freeze lui {targetCharacter.Name}.");
            targetCharacter.SendAdminMessage($"Ai primit freeze de la {adminAccount.Name} ({adminAccount.GameId}).");
        }

        [RequiresAdmin]
        [Command("unfreeze")]
        public void cmd_unfreeze(Player player, string target)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, target);
            if (targetCharacter == null) return;

            var adminAccount = AccountManager.GetAccount(player);
            targetCharacter.player.TriggerEvent("SERVER:CLIENT:UnfreezePlayer");

            targetCharacter.SendAdminMessage($"Ai primit unfreeze de la {adminAccount.Name} ({adminAccount.GameId}).");
            AdminManager.SendChatMessageToAdmins($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat unfreeze lui {targetCharacter.Name}.");
        }

    }
}
