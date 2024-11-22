using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Commands
{
    internal class ChatMessage : Script
    {
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
                    if(p != otherplayer)
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
