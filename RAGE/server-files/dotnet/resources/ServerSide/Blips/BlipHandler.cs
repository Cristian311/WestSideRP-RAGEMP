using System.Collections.Generic;
using GTANetworkAPI;
using System.IO;
namespace ServerSide.Blips
{
    internal class BlipHandler : Script
    {
        private static Blip blip2;
        private static List<BlipData> blips = new List<BlipData>();
        private string filePath = "json/blips/blips.json";
        private BlipHandler()
        {
            //test blip
            Vector3 blipPos = new Vector3(-418.45023, 1151.701, 326.87988);
            blip2 = NAPI.Blip.CreateBlip(11, blipPos, 1f, 2, "Hatz", 255, 0, false, 0, 0);

            blips.Add(new BlipData(blip2.Name, blipPos, blip2.Scale, byte.Parse(blip2.Color.ToString()), blip2.Sprite));
            //get existing blips
            string jsonString = File.ReadAllText(filePath);
            blips = NAPI.Util.FromJson<List<BlipData>>(jsonString);

            foreach(BlipData blip in blips)
            {
                NAPI.Blip.CreateBlip(blip.Sprite, blip.Position, blip.Scale, blip.Color, blip.Name, 255, 0, true, 0, NAPI.GlobalDimension);
            }
        }
        
        [Command("createblip")]
        private void CreateBlipCommand(Player player, string name, int sprite, byte color)
        {
            Blip createdBlip = NAPI.Blip.CreateBlip(sprite, player.Position, 1f, color, name, 255, 0, true, 0, NAPI.GlobalDimension);
            BlipData blipData = new BlipData(name, player.Position, 1f, color, uint.Parse(sprite.ToString()));
            blips.Add(blipData);
            File.WriteAllText(filePath, NAPI.Util.ToJson(blipData));
        }
    }
}
