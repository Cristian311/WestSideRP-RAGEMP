using GTANetworkAPI;
using ServerSide.Characters;
using ServerSide.Server.DB.CharacterDB;
using System;
using System.Collections.Generic;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Characters.Customization.Hair
{
    internal class HairManager : Script
    {
        private static Dictionary<Player, HairData> playerHair = new Dictionary<Player, HairData>();
        [RemoteEvent("CLIENT:SERVER:HairCustomizeUpdate")]
        private void OnHairCustomizeUpdate(Player player, string json)
        {
            var hairData = Newtonsoft.Json.JsonConvert.DeserializeObject<HairData>(json); //all character data vars are string
            NAPI.Util.ConsoleOutput(json);
            if (playerHair.ContainsKey(player))
                playerHair[player] = hairData;
            else
                playerHair.Add(player, hairData);
            //set open chest
            //player.SetClothes(3, 15, 0);
            //player.SetClothes(11, 15, 0);
            //player.SetClothes(8, 15, 0);
            //set hair
            try
            {
                UpdateHair(player, hairData);
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput("HairCustomize ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        }
        [RemoteEvent("CLIENT:SERVER:UpdateHair")]
        private void UpdateHairEvent(Player player, string json)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            var hairData = NAPI.Util.FromJson<HairData>(json);
            character.Hair = hairData;

            UpdateCharDataDBManager.UpdateCharacterHair(character);
        }
        public static void UpdateHair(Player player, HairData hairData)
        {
            var facialhair = CreateHeadOverlay(hairData.FacialHair.Value, hairData.FacialHair.Color, 0, 1f);
            var eyebrows = CreateHeadOverlay(hairData.Eyebrows.Value, hairData.Eyebrows.Color, 0, 1f);
            var bodyhair = CreateHeadOverlay(hairData.BodyHair.Value, hairData.BodyHair.Color, 0, 1f);
            player.SetHeadOverlay(1, facialhair);
            player.SetHeadOverlay(2, eyebrows);
            player.SetHeadOverlay(10, bodyhair);
            player.SetClothes(2, hairData.Hair.Value, 0);

            var character = CharacterManager.PlayerCharacterPair[player];
            player.SetCustomization(character.Gender, new HeadBlend(), character.Face.Eye.Color, hairData.Hair.Color, hairData.Hair.SecondColor, new float[20], new Dictionary<int, HeadOverlay>(), new Decoration[0]);
        }
        private static HeadOverlay CreateHeadOverlay(byte index, byte color, byte secondaryColor, float opacity)
        {
            return new HeadOverlay
            {
                Index = index,
                Color = color,
                SecondaryColor = secondaryColor,
                Opacity = opacity
            };
        }
    }
}
