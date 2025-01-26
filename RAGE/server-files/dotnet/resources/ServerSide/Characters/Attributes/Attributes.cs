using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Server.DB.CharacterDB;

namespace ServerSide.Characters.Attributes
{
    internal class Attributes : Script
    {
        [Command("attributes")]
        public static void cmd_attributes(Player player)
        {
            var darkmode = AccountManager.GetAccount(player).DarkMode;
            player.TriggerEvent("SERVER:CLIENT:OpenAttributesScreen", darkmode);
        }

        [RemoteEvent("CLIENT:SERVER:AttributesUpdate")]
        private void AttributesUpdate(Player player, string json)
        {
            var character = CharacterManager.GetCharacter(player);
            
            //first time attributes
            if (character.isCreated == false)
                character.isCreated = true;

            var charData = NAPI.Util.FromJson<CharacterAttributesData>(json);

            character.Age = charData.Age;
            character.CharacterDescription = charData.Description;
            character.OutfitDescription[character.CurrentOutfit] = charData.Outfit;
            character.TattooDescription = charData.Tattoos;
            UpdateCharDataDBManager.UpdateCharacter(character);
        }
        [Command("examine", Alias = "ex")]
        private void cmd_examine(Player player, string character)
        {
            var targetCharacter = CharacterManager.GetCharacterByNameOrId(player, character);
            if (targetCharacter == null) return;

            player.TriggerEvent("SERVER:CLIENT:OpenExamineScreen", targetCharacter.Age, targetCharacter.CharacterDescription, targetCharacter.OutfitDescription[targetCharacter.CurrentOutfit], targetCharacter.TattooDescription != string.Empty ? true : false, targetCharacter.Name);
        }

    }
}
