using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters.Attributes
{
    internal class Attributes : Script
    {
        [Command("attributes")]
        public static void cmd_attributes(Player player)
        {
            var darkmode = CharacterManager.GetCharacter(player).DarkMode;
            player.TriggerEvent("SERVER:CLIENT:OpenAttributesScreen", darkmode);
        }

        [RemoteEvent("CLIENT:SERVER:AttributesUpdate")]
        private void AttributesUpdate(Player player, string json)
        {
            var character = CharacterManager.GetCharacter(player);
            var charData = NAPI.Util.FromJson<CharacterAttributesData>(json);

            character.Age = charData.Age;
            character.CharacterDescription = charData.Description;
            character.OutfitDescription[character.CurrentOutfit] = charData.Outfit;
            character.TattooDescription = charData.Tattoos;
        }
        [Command("examine", Alias = "ex")]
        private void cmd_examine(Player player, string character)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, character);
            if (character == null) return;

            player.TriggerEvent("SERVER:CLIENT:OpenExamineScreen", targetCharacter.Age, targetCharacter.CharacterDescription, targetCharacter.OutfitDescription[targetCharacter.CurrentOutfit], targetCharacter.TattooDescription != string.Empty ? true : false, targetCharacter.Name);
        }
        
    }
}
