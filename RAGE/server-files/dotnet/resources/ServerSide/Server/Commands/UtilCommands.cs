using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerSide.Server.Commands
{
    internal class UtilCommands : Script
    {
        [Command("save")]
        private void cmd_save(Player player, string saveName)
        {
            string file_path = "C:\\Users\\sarof\\Desktop\\RAGE MP SERVERS\\GM DE RP .NET CORE 3.1\\RAGE\\server-files\\savedpos.txt";
            Vector3 playerPos = new Vector3(player.Position.X, player.Position.Y, player.Position.Z);
            Vector3 playerRot = new Vector3(player.Rotation.X, player.Rotation.Y, player.Rotation.Z);
            string format = $"{saveName} -\nPos :{playerPos.X}, {playerPos.Y}, {playerPos.Z}\nRot :{playerRot.X}, {playerRot.Y}, {playerRot.Z}\n";

            File.AppendAllText(file_path, format);
            NAPI.Util.ConsoleOutput("Saved position: " + format);
        }
        [Command("damage")]
        private void cmd_damage(Player player, int damage)
        {
            player.SendChatMessage($"Ai primit {damage} damage");
            player.Health -= damage;
        }
    }
}
