using GTANetworkAPI;
using ServerSide.Characters;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server
{
    internal class ChatlogHandler : Script
    {
        private static Dictionary<Player, List<string>> chatlogs = new Dictionary<Player, List<string>>();
        private static void readChatLogs(Player reader, string OtherName, List<string> chatlogs)
        {
            foreach(string log in chatlogs)
            {
                reader.SendChatMessage($"CHATLOG: {OtherName}: {log}");
            }
        }
        [RemoteEvent("CLIENT:SERVER:ChatLogger")]
        private void ChatLogger(Player player, string message)
        {
            if (chatlogs.ContainsKey(player))
            {
                chatlogs[player].Add(message);
            }
            else
                chatlogs.Add(player, new List<string> { message });
        }
        [RequiresAdmin]
        [Command("chatlogs")]
        private void ChatlogsCommand(Player player, string target)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var character = chars[0];
                if (chatlogs.ContainsKey(character.player) == false)
                {
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Acest player n-are nici un chat log.");
                    return;
                }
                readChatLogs(player, character.Name, chatlogs[character.player]);
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
