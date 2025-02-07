using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Characters.Customization.Clothes;
using ServerSide.Characters.Customization.Customize;
using ServerSide.Characters.Customization.Hair;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.DB.CharacterDB;



namespace ServerSide.Server.Commands
{
    internal class CustomizeCommands : Script
    {
        [Command("hair")]
        private void cmd_hair(Player player)
        {
            var character = CharacterManager.GetCharacter(player);
            var HairJSON = NAPI.Util.ToJson(character.Hair);
            player.TriggerEvent("SERVER:CLIENT:ShowHairScreen", HairJSON);
        }
        [Command("facefeatures")]
        private void cmd_facefeatures(Player player)
        {
            var character = CharacterManager.GetCharacter(player);
            var FaceJSON = NAPI.Util.ToJson(character.Face);

            player.PlayAnimation("amb@code_human_cross_road@male@idle_a", "idle_e", 1);
            player.TriggerEvent("SERVER:CLIENT:ShowCustomizeCharScreen", character.Gender, FaceJSON);
            player.TriggerEvent("SERVER:CLIENT:FaceCamera");
        }

        [RequiresAdmin]
        [Command("fixskin")]
        public static void cmd_fixskin(Player player)
        {
            var face = CharacterManager.GetCharacter(player).Face;
            CharacterCustomizeManager.UpdatePlayerFaceFeatures(player, face);
        }
        [Command("fixhair")]
        public static void cmd_fixhair(Player player)
        {
            var hair = CharacterManager.GetCharacter(player).Hair;
            HairManager.UpdateHair(player, hair);
        }

        [Command("changechar")]
        private void cmd_changechar(Player player)
        {
            var account = AccountManager.GetAccount(player);
            var character = CharacterManager.PlayerCharacterPair[player];

            var players = NAPI.Player.GetPlayersInRadiusOfPlayer(30, player);
            foreach (var p in players)
            {
                p.SendChatMessage($"{character.Name} si-a schimbat characterul.");
            }
            CharacterManager.RemoveCharacter(character);
            CharacterManager.ShowChangeCharacterScreen(account);

            UpdateCharDataDBManager.UpdateCharacter(character);

        }

        [Command("clothes")]
        private void cmd_clothes(Player player)
        {
            var account = AccountManager.GetAccount(player);
            var character = CharacterManager.GetCharacter(player);
            ClothesTextureData clothesNames = new ClothesTextureData();
            var gender = character.Gender ? "male" : "female";
            clothesNames.SetGender(gender);
            var itemMaxValuesJSON = NAPI.Util.ToJson(clothesNames.results);
            var clothes = character.Clothes[character.CurrentOutfit];
            var clothesJson = NAPI.Util.ToJson(clothes);
            player.TriggerEvent("SERVER:CLIENT:OpenClothesScreen", clothesJson, character.CurrentOutfit);
            player.TriggerEvent("SERVER:CLIENT:UpdateDataMaxValues", itemMaxValuesJSON);
            ClothesManager.SendOutfitNames(player);

        }
        [Command("shirt")]
        private void cmd_shirt(Player player, string toggle)
        {
            var character = CharacterManager.GetCharacter(player);
            if (toggle == "off")
            {
                player.SetClothes(3, 15, 0); // torso
                player.SetClothes(11, 15, 0); // shirt
                player.SetClothes(8, 15, 0); // undershirt
            }
            if (toggle == "on")
            {
                var clothes = character.Clothes[character.CurrentOutfit];
                player.SetClothes(3, clothes.Torso.Drawable, clothes.Torso.Texture); // torso
                player.SetClothes(11, clothes.Shirts.Drawable, clothes.Shirts.Texture); // shirt
                player.SetClothes(8, clothes.Undershirt.Drawable, clothes.Undershirt.Texture); // undershirt
            }
        }
        [Command("pants")]
        private void cmd_pants(Player player, string toggle)
        {
            var character = CharacterManager.GetCharacter(player);
            if (toggle == "off")
            {
                player.SetClothes(4, 61, 0); // legs
            }
            if (toggle == "on")
            {
                var clothes = character.Clothes[character.CurrentOutfit];
                player.SetClothes(4, clothes.Legs.Drawable, clothes.Legs.Texture); // legs
            }
        }
        [Command("shoes")]
        private void cmd_shoes(Player player, string toggle)
        {
            var character = CharacterManager.GetCharacter(player);
            if (toggle == "off")
            {
                player.SetClothes(6, 34, 0); // shoes
            }
            if (toggle == "on")
            {
                var clothes = character.Clothes[character.CurrentOutfit];
                player.SetClothes(6, clothes.Shoes.Drawable, clothes.Shoes.Texture); // shoes
            }
        }
        [Command("cap")]
        private void cmd_cap(Player player, string toggle)
        {
            var character = CharacterManager.GetCharacter(player);
            if (toggle == "off")
            {
                player.ClearAccessory(0); // cap
            }
            if (toggle == "on")
            {

                var clothes = character.Clothes[character.CurrentOutfit];
                player.SetAccessories(0, clothes.Helmets.Drawable, clothes.Helmets.Texture); // cap
            }
        }
    }
}
