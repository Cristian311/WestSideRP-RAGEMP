using GTANetworkAPI;
using ServerSide.Characters;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server
{
    internal class ChatMessage : Script
    {
        [ServerEvent(Event.ChatMessage)]
        private void playerChat(Player player, string message)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            message = StringFormatter.FormatStringForChatMessage(message);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{name} spune: {message}");
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
        [Command("low", GreedyArg = true)]
        private void lowCommand(Player player, string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            var name = CharacterManager.PlayerCharacterPair[player].Name;
            foreach (var p in players)
            {
                p.SendChatMessage($"{Colors.COLOR_CHAT_LOW}{name} spune(încet): {action}");
            }
        }
    }
}
