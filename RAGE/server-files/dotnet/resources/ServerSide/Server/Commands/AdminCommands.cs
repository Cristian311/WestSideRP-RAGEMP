using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Util;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Server.Commands
{
    internal class AdminCommands : Script
    {
        AccountManager accountManager = new AccountManager();

        [RequiresAdmin(5)]
        [Command("makeadmin")]
        private void MakeAdminCommand(Player player, string target, int adminLevel = 1)
        {
            var adminAccount = AccountManager.PlayerAccountPairs[player];
            var targetCharacter = CharacterManager.GetCharacterByName(player, target);
            if (targetCharacter == null) return;

            var targetAccount = AccountManager.PlayerAccountPairs[targetCharacter.player];
            AdminManager.SendChatMessageToAdmins($"Adminul {adminAccount.AdminLevel} {adminAccount.Name} l-a facut admin {adminLevel} pe {targetAccount.Name}");
            targetAccount.AdminLevel = adminLevel;
        }
        [RequiresAdmin]
        [Command("createveh")]
        public void CreateVehicleCommand(Player player, string car, int color1 = 0, int color2 = 0)
        {
            VehicleHash busModel = NAPI.Util.VehicleNameToModel(car); //Gets the hash of bus model
            NAPI.Vehicle.CreateVehicle(busModel, player.Position, 0, color1, color2); //Creates a bus on the player's position
        }
        [RequiresAdmin]
        [Command("die")]
        public void die_cmd(Player player)
        {
            player.Kill();
        }
        [RequiresAdmin]
        [Command("gotoxyz")]
        public void GotoXYZ_Command(Player player, float x, float y, float z)
        {
            Vector3 position = new Vector3(x, y, z);
            player.Position = position;
        }
        [RequiresAdmin]
        [Command("weapon")]
        public void WeaponCommand(Player player, string weapon, int ammo)
        {
            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(weapon);
            player.GiveWeapon(weaponHash, ammo);
        }

        [RequiresAdmin]
        [Command("weather")]
        public void WeatherCommand(Player player, string weather)
        {

            NAPI.World.SetWeather(weather);
            player.SendNotification($"Set weather to {weather}.");
        }

        [RequiresAdmin]
        [Command("time")]
        public void TimeCommand(Player player, int hour)
        {
            NAPI.World.SetTime(hour, 0, 0);
            player.SendNotification($"Set time to {hour}.");
        }

        // MODERATION COMMANDS
        [RequiresAdmin]
        [Command("kick", GreedyArg = true)]
        public void KickPlayerCommand(Player player, string target, string reason)
        {
            var character = CharacterManager.GetCharacterByName(player, target);
            if (character == null) return;

            var adminAccount = AccountManager.PlayerAccountPairs[player];
            var targetAccount = CharacterManager.CharacterAccountPair[character];

            targetAccount.player.Kick($"You were kicked by an admin. Reason: {reason}");
            NAPI.Chat.SendChatMessageToAll($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat kick lui {targetAccount.currentCharacter.Name} Motiv: {reason}");
        }

        [RequiresAdmin]
        [Command("ban")]
        public void BanPlayerCommand(Player player, string target, string reason)
        {
            var character = CharacterManager.GetCharacterByName(player, target);
            if (character == null) return;

            var adminAccount = AccountManager.PlayerAccountPairs[player];
            var targetAccount = CharacterManager.CharacterAccountPair[character];

            targetAccount.player.Ban($"You were banned by an admin. Reason: {reason}");
            NAPI.Chat.SendChatMessageToAll($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat ban lui {targetAccount.currentCharacter.Name} Motiv: {reason}");
        }

        [RequiresAdmin]
        [Command("mute")]
        public void MutePlayerCommand(Player player, string target, string reason)
        {
            var character = CharacterManager.GetCharacterByName(player, target);
            if (character == null) return;

            var adminAccount = AccountManager.PlayerAccountPairs[player];
            var targetAccount = CharacterManager.CharacterAccountPair[character];

            targetAccount.Muted = true;
            NAPI.Chat.SendChatMessageToAll($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat mute lui {targetAccount.currentCharacter.Name} Motiv: {reason}");
        }

        [RequiresAdmin]
        [Command("unmute")]
        public void UnmutePlayerCommand(Player player, string target)
        {
            var character = CharacterManager.GetCharacterByName(player, target);
            if (character == null) return;

            var adminAccount = AccountManager.PlayerAccountPairs[player];
            var targetAccount = CharacterManager.CharacterAccountPair[character];

            targetAccount.Muted = false;
            NAPI.Chat.SendChatMessageToAll($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat unmute lui {targetAccount.currentCharacter.Name}");
        }

        // TELEPORT COMMANDS
        [RequiresAdmin]
        [Command("goto")]
        public void GotoPlayerCommand(Player player, string target)
        {
            var character = CharacterManager.GetCharacterByName(player, target);
            if (character == null) return;

            var adminAccount = AccountManager.PlayerAccountPairs[player];
            player.Position = character.Account.player.Position;

            AdminManager.SendChatMessageToAdmins($"{Colors.ERROR}Admin {adminAccount.Name} s-a teleportat la {character.Name}");
        }

        [RequiresAdmin]
        [Command("bring")]
        public void BringPlayerCommand(Player player, string target)
        {
            var character = CharacterManager.GetCharacterByName(player, target);
            if (character == null) return;

            var adminAccount = AccountManager.PlayerAccountPairs[player];
            character.Account.player.Position = player.Position;

            AdminManager.SendChatMessageToAdmins($"{Colors.ERROR}Admin {adminAccount.Name} l-a teleportat pe {character.Name} la el");
        }

        // FREEZE/UNFREEZE COMMANDS
        [RequiresAdmin]
        [Command("freeze")]
        public void FreezePlayerCommand(Player player, string target)
        {
            var character = CharacterManager.GetCharacterByName(player, target);
            if (character == null) return;

            var adminAccount = AccountManager.PlayerAccountPairs[player];
            NAPI.Player.FreezePlayerTime(character.Account.player, true);

            AdminManager.SendChatMessageToAdmins($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat freeze lui {character.Name}");
        }
        [RequiresAdmin]
        [Command("aduty")]
        private void adutyCommand(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            if(account.AdminDuty == false)
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
        [Command("unfreeze")]
        public void UnfreezePlayerCommand(Player player, string target)
        {
            var character = CharacterManager.GetCharacterByName(player, target);
            if (character == null) return;

            var adminAccount = AccountManager.PlayerAccountPairs[player];
            NAPI.Player.FreezePlayerTime(character.Account.player, false);

            AdminManager.SendChatMessageToAdmins($"{Colors.ERROR}Admin {adminAccount.Name} i-a dat unfreeze lui {character.Name}");
        }

    }
}
