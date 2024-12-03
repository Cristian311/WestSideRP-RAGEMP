using GTANetworkAPI;
using Org.BouncyCastle.Asn1.IsisMtt.Ocsp;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Commands.CommandAttributes;
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

        [RequiresAdmin]
        [Command("createveh")]
        public void CreateVehicleCommand(Player player, string car)
        {
            VehicleHash busModel = NAPI.Util.VehicleNameToModel(car); //Gets the hash of bus model
            NAPI.Vehicle.CreateVehicle(busModel, player.Position, 0, 0, 0); //Creates a bus on the player's position

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

        ////MODERATION COMMANDS
        [RequiresAdmin]
        [Command("kick")]
        public void KickPlayerCommand(Player player, string target, string reason)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
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

        [RequiresAdmin]
        [Command("ban")]
        public void BanPlayerCommand(Player player, string target, string reason)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
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

        [RequiresAdmin]
        [Command("mute")]
        public void MutePlayerCommand(Player player, string target, string reason)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
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

        [RequiresAdmin]
        [Command("unmute")]
        public void UnmutePlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
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
        [RequiresAdmin]
        [Command("goto")]
        public void GotoPlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                player.Position = character.Account.player.Position;
                //TODO LOGIC BAN DATABASE
                AdminManager.SendChatMessageToAdmins($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} s-a teleportat la {character.Name}");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }

        [RequiresAdmin]
        [Command("bring")]
        public void BringPlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                character.Account.player.Position = player.Position;
                //TODO LOGIC BAN DATABASE
                AdminManager.SendChatMessageToAdmins($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} l-a teleportat pe {character.Name} la el");
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
        [RequiresAdmin]
        [Command("freeze")]
        public void FreezePlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                NAPI.Player.FreezePlayerTime(player, true);
                //TODO LOGIC BAN DATABASE
                AdminManager.SendChatMessageToAdmins($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat freeze lui {character.Name}");
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
        [RequiresAdmin]
        [Command("unfreeze")]
        public void UnfreezePlayerCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                var AdminAccount = AccountManager.PlayerAccountPairs[player];
                var TargetAccount = CharacterManager.CharacterAccountPair[character];

                NAPI.Player.FreezePlayerTime(player, false);
                
                //TODO LOGIC BAN DATABASE
                AdminManager.SendChatMessageToAdmins($"{Colors.COLOR_CHAT_ERROR}Admin {AdminAccount.Name} i-a dat unfreeze lui {character.Name}");
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
