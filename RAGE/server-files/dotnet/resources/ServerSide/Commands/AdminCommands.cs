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
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                TargetAccount.player.Kick($"You were kicked by an admin. Reason: {reason}");
                NAPI.Chat.SendChatMessageToAll($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat kick lui {TargetAccount.currentCharacter.Name} Motiv: {reason}");
            }
            else
            {
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
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                TargetAccount.player.Ban($"You were banned by an admin. Reason: {reason}");
                //TODO LOGIC BAN DATABASE
                NAPI.Chat.SendChatMessageToAll($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat ban lui {TargetAccount.currentCharacter.Name} Motiv: {reason}");
            }
            else
            {
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
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                TargetAccount.Muted = true;
                //TODO LOGIC MUTE DATABASE
                NAPI.Chat.SendChatMessageToAll($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat mute lui {TargetAccount.currentCharacter.Name} Motiv: {reason}");
            }
            else
            {
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
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                TargetAccount.Muted = false;
                //TODO LOGIC MUTE DATABASE
                NAPI.Chat.SendChatMessageToAll($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat unmute lui {TargetAccount.currentCharacter.Name}");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        //TELEPORT COMMANDS
        [Command("goto")]
        public void GotoPlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                player.Position = character.Account.player.Position;
                //TODO LOGIC BAN DATABASE
                AdminManager.sendChatMessageToAdmins($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} s-a teleportat la {character.Name}");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        [Command("bring")]
        public void BringPlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                character.Account.player.Position = player.Position;
                //TODO LOGIC BAN DATABASE
                AdminManager.sendChatMessageToAdmins($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} l-a teleportat pe {character.Name} la el");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }

        //FREEEZE
        [Command("freeze")]
        public void FreezePlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                NAPI.Player.FreezePlayerTime(player, true);
                //TODO LOGIC BAN DATABASE
                AdminManager.sendChatMessageToAdmins($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat freeze lui {character.Name}");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        //needs fixed
        [Command("unfreeze")]
        public void UnfreezePlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                if (!AdminManager.CheckIfAdminAccess(player, 1))
                    return;
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                NAPI.Player.FreezePlayerTime(player, false);
                
                //TODO LOGIC BAN DATABASE
                AdminManager.sendChatMessageToAdmins($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat unfreeze lui {character.Name}");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
    }
}
