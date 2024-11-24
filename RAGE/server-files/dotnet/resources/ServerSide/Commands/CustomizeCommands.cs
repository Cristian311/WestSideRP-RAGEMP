using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Clothes;
using ServerSide.DB.CharacterDB;



namespace ServerSide.Commands
{
    internal class CustomizeCommands : Script
    {
        [Command("hair")]
        private void hairCommand(Player player)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            var HairJSON = NAPI.Util.ToJson(character.Hair);
            player.TriggerEvent("SERVER:CLIENT:showHairScreen", HairJSON);
        }
        [Command("facefeatures")]
        private void facefeaturesCommand(Player player)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            var FaceJSON = NAPI.Util.ToJson(character.Face);
            player.TriggerEvent("SERVER:CLIENT:showCustomizeCharScreen", FaceJSON);
        }
        [Command("changechar")]
        private void changecharCommand(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            var character = CharacterManager.PlayerCharacterPair[player];
            string username = account.Name;

            CharacterManager.GetNameMoneyJson(account, out string namesJSON, out string moneyJSON);
            UpdateCharDataDBManager.UpdateCharacter(character);
            //display character interface with new character data
            player.TriggerEvent("SERVER:CLIENT:showCharInterface");
            player.TriggerEvent("SERVER:CLIENT:displayAccountInfo", username, namesJSON, moneyJSON);
            UpdateCharDataDBManager.UpdateCharacter(character);
        }
        [Command("clothes")]
        private void ClothesCommand(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            var character = CharacterManager.PlayerCharacterPair[player];
            ClothesTextureData clothesNames = new ClothesTextureData();
            var gender = character.Gender ? "male" : "female";
            clothesNames.SetGender(gender);
            var itemMaxValuesJSON = NAPI.Util.ToJson(clothesNames.results);
            player.TriggerEvent("SERVER:CLIENT:openClothesScreen");
            player.TriggerEvent("SERVER:CLIENT:UpdateDataMaxValues", itemMaxValuesJSON);
            
        }
        [Command("shirt")]
        private void ShirtCommand(Player player, string toggle)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (toggle == "off")
            {
                player.SetClothes(3, 15, 0); // torso
                player.SetClothes(11, 15, 0); // shirt
                player.SetClothes(8, 15, 0); // undershirt
            }
            if(toggle == "on")
            {
                int index = character.currentOutfit;
                player.SetClothes(3, character.Clothes[index].Torso.Drawable, character.Clothes[index].Torso.Texture); // torso
                player.SetClothes(11, character.Clothes[index].Shirts.Drawable, character.Clothes[index].Shirts.Texture); // shirt
                player.SetClothes(8, character.Clothes[index].Undershirt.Drawable, character.Clothes[index].Undershirt.Texture); // undershirt
            }
        }
    }
}
