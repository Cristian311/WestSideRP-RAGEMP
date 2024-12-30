using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ServerSide.Server.Commands
{
    internal class ChatMessage : Script
    {
        private static Dictionary<Player, Stopwatch> PlayerStopwatchAmePair = new Dictionary<Player, Stopwatch>();

        //UPDATE NAMETAG
        [ServerEvent(Event.Update)]
        private void UpdatePlayerNametag()
        {
            foreach(Player player  in PlayerStopwatchAmePair.Keys)
            {
                if (PlayerStopwatchAmePair[player].Elapsed.Seconds > 5)
                {
                    var character = CharacterManager.PlayerCharacterPair[player];
                    player.Name = character.Name;
                    PlayerStopwatchAmePair.Remove(player);
                }
            }
        }

        [Command("ame", GreedyArg = true)]
        public static void ameCommand(Player player, string action)
        {
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            string newName = $"~p~{name} {action}\n~w~{name}";
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
            var account = AccountManager.PlayerAccountPairs[player];
            if (account.Muted == true)
                player.SendChatMessage($"{Colors.COLOR_CHAT_MUTED}You are muted and can't send a message");
            else
            {
                var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
                message = StringFormatter.FormatStringForChatMessage(message);
                var name = CharacterManager.PlayerCharacterPair[player].Name;
                foreach (var p in players)
                {
                    p.SendChatMessage($"{name} spune: {message}");
                }
            }
        }
        [Command("b", GreedyArg = true)]
        private void bCommand(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.COLOR_CHAT_DARK_GRAY} (( {name}: {action} )) ");
            }
        }
        [Command("to")]
        private void cmd_to(Player player, string target, string message)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                var otherplayer = CharacterManager.CharacterAccountPair[character].player;

                var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
                var name = CharacterManager.PlayerCharacterPair[player].Name;
                foreach (var p in players)
                {
                    if (p != otherplayer)
                        p.SendChatMessage($"{Colors.COLOR_CHAT_LOW}{name} spune lui {character.Name}: {message}");
                }
                otherplayer.SendChatMessage($"{Colors.COLOR_CHAT_PARTY}[!]{Colors.COLOR_CHAT_LOW}{name} spune lui {character.Name}: {message}");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        [Command("w")]
        private void cmd_w(Player player, string target, string message)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                var otherplayer = CharacterManager.CharacterAccountPair[character].player;

                var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
                var name = CharacterManager.PlayerCharacterPair[player].Name;

                otherplayer.SendChatMessage($"{Colors.COLOR_CHAT_WHISPER}{name} spune(soapta): {message}");
                player.SendChatMessage($"{Colors.COLOR_CHAT_WHISPER}{name} spune(soapta): {message}");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        [Command("lowto")]
        private void cmd_lowto(Player player, string target, string message)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];

                var players = NAPI.Player.GetPlayersInRadiusOfPlayer(15, player);
                var otherplayer = CharacterManager.CharacterAccountPair[character].player;

                var name = CharacterManager.PlayerCharacterPair[player].Name;
                foreach (var p in players)
                {
                    if (p != otherplayer)
                        p.SendChatMessage($"{Colors.COLOR_CHAT_LOW}{name} spune(încet) lui {character.Name}: {message}");
                }
                otherplayer.SendChatMessage($"{Colors.COLOR_CHAT_PARTY}[!]{Colors.COLOR_CHAT_LOW}{name} spune(încet) lui {character.Name}: {message}");
            }
            else
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        [Command("melow", GreedyArg = true)]
        private void cmd_melow(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(15, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.COLOR_CHAT_ME}* {name} {action}");
            }
        }
        

        [Command("me", GreedyArg = true)]
        private void meCommand(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.COLOR_CHAT_ME}* {name} {action}");
            }
        }

        [Command("do", GreedyArg = true)]
        private void doCommand(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.COLOR_CHAT_ME}* {action} ({CharacterManager.PlayerCharacterPair[player].Name}) ");
            }
        }
        [Command("dolow", GreedyArg = true)]
        private void dolowCommand(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(15, player);
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.COLOR_CHAT_ME}* {action} ({CharacterManager.PlayerCharacterPair[player].Name}) ");
            }
        }
        [Command("low", GreedyArg = true)]
        private void lowCommand(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(15, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.COLOR_CHAT_LOW}{name} spune(încet): {action}");
            }
        }

    }
}
