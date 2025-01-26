using GTANetworkAPI;
using GTANetworkMethods;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Util;
using System.Collections.Generic;
using System.Diagnostics;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Server.Commands
{
    internal class ChatMessage : Script
    {
        private static Dictionary<Player, Stopwatch> PlayerStopwatchAmePair = new Dictionary<Player, Stopwatch>();

        //UPDATE NAMETAG
        [ServerEvent(Event.Update)]
        private void UpdatePlayerNametag()
        {
            foreach (Player player in PlayerStopwatchAmePair.Keys)
            {
                if (PlayerStopwatchAmePair[player].Elapsed.Seconds > 5)
                {
                    var character = CharacterManager.PlayerCharacterPair[player];
                    player.Name = character.Name;
                    PlayerStopwatchAmePair.Remove(player);
                }
            }
        }

        [Command("ame", "/ame [message]", GreedyArg = true)]
        public static void cmd_ame(Player player, string action, bool chatMessage = false)
        {
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            string newName = StringFormatter.FormatStringForAme(name, action);

            player.Name = newName;

            if (PlayerStopwatchAmePair.ContainsKey(player))
                PlayerStopwatchAmePair[player].Restart();
            else
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                PlayerStopwatchAmePair.Add(player, stopwatch);
            }
        }
        //
        [ServerEvent(Event.ChatMessage)]
        private void playerChat(Player player, string message)
        {
            var account = AccountManager.GetAccount(player);
            if (account.Muted == true)
            {
                player.SendChatMessage($"{Colors.MUTED}You are muted and can't send a message");
                return;
            }
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            message = StringFormatter.FormatStringForChatMessage(message);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{name} spune: {message}");
            }
        }

        [Command("b", GreedyArg = true)]
        private void cmd_b(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.GRAY} (( {name}: {action} )) ");
            }
        }

        [Command("to")]
        private void cmd_to(Player player, string target, string message)
        {
            var character = CharacterManager.GetCharacterByNameOrId(target);
            if (character == null) return;

            var otherplayer = character.player;

            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            var name = CharacterManager.GetCharacter(player).Name;

            foreach (var p in players)
            {
                if (p != otherplayer)
                    p.SendChatMessage($"{Colors.LOW}{name} spune lui ({character.Name}): {message}");
            }

            otherplayer.SendChatMessage($"{Colors.PARTY}[!]{Colors.LOW}{name} spune lui ({character.Name}): {message}");
        }
        [Command("w")]
        private void cmd_w(Player player, string target, string message)
        {
            var character = CharacterManager.GetCharacterByNameOrId(player, target);
            if (character == null) return;

            var otherplayer = character.player;

            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;

            if (players.Contains(otherplayer))
            {
                otherplayer.SendChatMessage($"{Colors.WHISPER}{name} spune(soapta): {message}");
                player.SendChatMessage($"{Colors.WHISPER}{name} spune(soapta): {message}");
            }
            else
            {
                player.SendChatMessage($"{Colors.ERROR}Acel player nu este langa tine");
            }

        }
        [Command("pm")]
        private void cmd_pm(Player player, string target, string message)
        {
            var character = CharacterManager.GetCharacterByNameOrId(player, target);
            if (character == null) return;

            var otherplayer = character.player;

            var name = CharacterManager.PlayerCharacterPair[player].Name;

            otherplayer.SendChatMessage($"{Colors.YELLOW}(( {name}: {message} ))");
            player.SendChatMessage($"{Colors.YELLOW}(( {name}: {message} ))");

        }
        [Command("lowto", Alias = "lto")]
        private void cmd_lowto(Player player, string target, string message)
        {
            var character = CharacterManager.GetCharacterByNameOrId(player, target);
            if (character == null) return;

            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(15, player);
            var playerCharacter = CharacterManager.GetCharacter(player);

            var name = CharacterManager.GetCharacter(player).Name;
            foreach (var p in players)
            {
                if (p != playerCharacter.player)
                    p.SendChatMessage($"{Colors.LOW}{name} spune(încet) lui {character.Name}: {message}");
            }
            playerCharacter.SendChatMessage($"{Colors.PARTY}[!]{Colors.LOW}{name} spune(încet) lui {character.Name}: {message}");
        }
        [Command("melow", GreedyArg = true)]
        private void cmd_melow(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(15, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.ME}* {name} {action}");
            }
        }


        [Command("me", GreedyArg = true)]
        private void cmd_me(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.ME}* {name} {action}");
            }
            cmd_ame(player, action, false);
        }

        [Command("do", GreedyArg = true)]
        private void cmd_do(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.ME}* {action} ({CharacterManager.PlayerCharacterPair[player].Name}) ");
            }
        }
        [Command("dolow", GreedyArg = true)]
        private void cmd_dolow(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(15, player);
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.ME}* {action} ({CharacterManager.PlayerCharacterPair[player].Name}) ");
            }
        }
        [Command("low", Alias = "l", GreedyArg = true)]
        private void cmd_low(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(15, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.LOW}{name} spune(încet): {action}");
            }
        }

        [Command("lower", GreedyArg = true)]
        private void cmd_lower(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(7, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.LOW}{name} spune(încet): {action}");
            }
        }
    }
}
