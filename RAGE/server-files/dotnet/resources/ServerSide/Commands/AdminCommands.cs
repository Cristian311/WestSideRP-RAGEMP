using GTANetworkAPI;
using Org.BouncyCastle.Asn1.IsisMtt.Ocsp;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Util;
using ServerSide.Util.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Commands
{
    internal class AdminCommands : Script
    {
        AccountManager accountManager = new AccountManager();
        [Command("createveh")]
        public void CreateVehicleCommand(Player player, string car)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            VehicleHash busModel = NAPI.Util.VehicleNameToModel(car); //Gets the hash of bus model
            NAPI.Vehicle.CreateVehicle(busModel, player.Position, 0, 0, 0); //Creates a bus on the player's position

        }
        [Command("weapon")]
        public void WeaponCommand(Player player, string weapon, int ammo)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(weapon);
            player.GiveWeapon(weaponHash, ammo);
        }
        [Command("weather")]
        public void cmd_weather(Player player, string weather)
        {
            if (!AdminManager.CheckIfAdminAccess(player,1))
                return;
            NAPI.World.SetWeather(weather);
            player.SendNotification($"Set weather to {weather}.");
        }
        [Command("time")]
        public void cmd_time(Player player, int hour)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            NAPI.World.SetTime(hour, 0, 0);
            player.SendNotification($"Set time to {hour}.");
        }

        ////MODERATION COMMANDS
        [Command("kick")]
        public void KickPlayerCommand(Player player, string target, string reason)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                player.SendChatMessage("hatz");
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var KickedAccount = CharacterManager.CharacterAccountPair[character];

                KickedAccount.player.Kick($"You were kicked by an admin. Reason: {reason}");
                NAPI.Chat.SendChatMessageToAll($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat kick lui {KickedAccount.currentCharacter.Name} Motiv: {reason}");
            }
            else
            {
                player.SendChatMessage("hatz2");
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        [Command("ban")]
        public void BanPlayerCommand(Player player, string target, string reason)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                player.SendChatMessage("hatz");
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var KickedAccount = CharacterManager.CharacterAccountPair[character];

                KickedAccount.player.Ban($"You were banned by an admin. Reason: {reason}");
                //TODO LOGIC BAN DATABASE
                NAPI.Chat.SendChatMessageToAll($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat ban lui {KickedAccount.currentCharacter.Name} Motiv: {reason}");
            }
            else
            {
                player.SendChatMessage("hatz2");
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        [Command("mute")]
        public void MutePlayerCommand(Player player, string target, string reason)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                player.SendChatMessage("hatz");
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var KickedAccount = CharacterManager.CharacterAccountPair[character];

                KickedAccount.Muted = true;
                //TODO LOGIC BAN DATABASE
                NAPI.Chat.SendChatMessageToAll($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat mute lui {KickedAccount.currentCharacter.Name} Motiv: {reason}");
            }
            else
            {
                player.SendChatMessage("hatz2");
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        [Command("unmute")]
        public void UnmutePlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                player.SendChatMessage("hatz");
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var KickedAccount = CharacterManager.CharacterAccountPair[character];

                KickedAccount.Muted = false;
                //TODO LOGIC BAN DATABASE
                NAPI.Chat.SendChatMessageToAll($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat mute lui {KickedAccount.currentCharacter.Name}");
            }
            else
            {
                player.SendChatMessage("hatz2");
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
    }
}
