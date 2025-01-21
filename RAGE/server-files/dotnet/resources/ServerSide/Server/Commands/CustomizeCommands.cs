using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Characters.Customization.Clothes;
using ServerSide.Characters.Customization.Customize;
using ServerSide.Server.DB.CharacterDB;
using ServerSide.Util;



namespace ServerSide.Server.Commands
{
    internal class CustomizeCommands : Script
    {
        [Command("hair")]
        private void hairCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var HairJSON = NAPI.Util.ToJson(character.Hair);
            player.TriggerEvent("SERVER:CLIENT:ShowHairScreen", HairJSON);
        }
        [Command("facefeatures")]
        private void facefeaturesCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var FaceJSON = NAPI.Util.ToJson(character.Face);

            player.PlayAnimation("amb@code_human_cross_road@male@idle_a", "idle_e", 1);
            player.TriggerEvent("SERVER:CLIENT:ShowCustomizeCharScreen", character.Gender, FaceJSON);
            player.TriggerEvent("SERVER:CLIENT:FaceCamera");
        }

        [Command("fixskin")]
        public static void fixskinCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            CharacterCustomizeManager.UpdatePlayerFaceFeatures(player, character.Face);
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
            player.TriggerEvent("SERVER:CLIENT:ShowCharInterface");
            player.TriggerEvent("SERVER:CLIENT:DisplayAccountInfo", username, namesJSON, moneyJSON);
            UpdateCharDataDBManager.UpdateCharacter(character);
            player.Dimension = (uint)account.GameId;
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
            var clothes = character.Clothes[character.currentOutfit];
            var clothesJson = NAPI.Util.ToJson(clothes);
            player.TriggerEvent("SERVER:CLIENT:OpenClothesScreen", clothesJson, character.currentOutfit);
            player.TriggerEvent("SERVER:CLIENT:UpdateDataMaxValues", itemMaxValuesJSON);
            ClothesManager.SendOutfitNames(player);

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
            if (toggle == "on")
            {
                var clothes = character.Clothes[character.currentOutfit];
                player.SetClothes(3, clothes.Torso.Drawable, clothes.Torso.Texture); // torso
                player.SetClothes(11, clothes.Shirts.Drawable, clothes.Shirts.Texture); // shirt
                player.SetClothes(8, clothes.Undershirt.Drawable, clothes.Undershirt.Texture); // undershirt
            }
        }
        [Command("pants")]
        private void PantsCommand(Player player, string toggle)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (toggle == "off")
            {
                player.SetClothes(4, 61, 0); // legs
            }
            if (toggle == "on")
            {
                var clothes = character.Clothes[character.currentOutfit];
                player.SetClothes(4, clothes.Legs.Drawable, clothes.Legs.Texture); // legs
            }
        }
        [Command("shoes")]
        private void ShoesCommand(Player player, string toggle)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (toggle == "off")
            {
                player.SetClothes(6, 34, 0); // shoes
            }
            if (toggle == "on")
            {
                var clothes = character.Clothes[character.currentOutfit];
                player.SetClothes(6, clothes.Shoes.Drawable, clothes.Shoes.Texture); // shoes
            }
        }
        [Command("cap")]
        private void CapCommand(Player player, string toggle)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (toggle == "off")
            {
                player.ClearAccessory(0); // cap
            }
            if (toggle == "on")
            {

                var clothes = character.Clothes[character.currentOutfit];
                player.SetAccessories(0, clothes.Helmets.Drawable, clothes.Helmets.Texture); // cap
            }
        }
    }
}
