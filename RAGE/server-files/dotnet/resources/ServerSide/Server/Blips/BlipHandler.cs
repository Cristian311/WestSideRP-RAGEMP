using System.Collections.Generic;
using GTANetworkAPI;
using System.IO;
using ServerSide.Server.DB;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Admin;
using ServerSide.Accounts;
using ServerSide.Util;
namespace ServerSide.Server.Blips
{
    internal class BlipHandler : Script
    {
        private static Blip blip2;
        private static List<BlipData> blips = new List<BlipData>();
        private static IdManager<BlipData> blipIdManager = new IdManager<BlipData>();
        private static string filePath = "json/blips/blips.json";
        private BlipHandler()
        {
            //test blip
            Vector3 blipPos = new Vector3(-418.45023, 1151.701, 326.87988);
            blip2 = NAPI.Blip.CreateBlip(11, blipPos, 1f, 2, "Hatz", 255, 0, false, 0, 0);

            blips.Add(new BlipData(blip2.Name, blipPos, blip2.Scale, byte.Parse(blip2.Color.ToString()), blip2.Sprite));

            //get existing blips
            blips = BlipDbManager.GetBlips();
            if(blips != null)
                foreach (BlipData blip in blips)
                {
                    NAPI.Blip.CreateBlip(blip.Sprite, blip.Position, blip.Scale, blip.Color, blip.Name, 255, 0, true, 0, NAPI.GlobalDimension);
                    blipIdManager.SetIdBusy(blip.Id);
                }
        }
        [RequiresAdmin]
        [Command("createblip")]
        public static void CreateBlipCommand(Player player, string name, int sprite, byte color)
        {
            Blip createdBlip = NAPI.Blip.CreateBlip(sprite, player.Position, 1f, color, name, 255, 0, true, 0, NAPI.GlobalDimension);
            BlipData blipData = new BlipData(name, player.Position, 1f, color, uint.Parse(sprite.ToString()));
            blipData.Id = blipIdManager.AssignNewId(blipData);
            blips.Add(blipData);
            BlipDbManager.CreateBlip(blipData);
            var AdminAccount = AccountManager.PlayerAccountPairs[player];
            AdminManager.SendChatMessageToAdmins($"{Colors.COLOR_CHAT_ADMIN}{AdminAccount.Name} created Blip {name} ID {blipData.Id} at {player.Position.ToString()}");
        }
    }
}
