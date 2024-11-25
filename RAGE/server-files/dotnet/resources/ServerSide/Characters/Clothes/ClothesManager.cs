using GTANetworkAPI;
using Newtonsoft.Json;
using ServerSide.DB.CharacterDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ServerSide.Characters.Clothes
{
    internal class ClothesManager : Script
    {
        private static string defaultClothesJSON = "{\"id\":2,\"masks\":{\"drawable\":0,\"texture\":0},\"torsos\":{\"drawable\":0,\"texture\":0},\"legs\":{\"drawable\":0,\"texture\":0},\"bags\":{\"drawable\":0,\"texture\":0},\"shoes\":{\"drawable\":0,\"texture\":0},\"accessories\":{\"drawable\":0,\"texture\":0},\"undershirts\":{\"drawable\":0,\"texture\":0},\"bodyArmor\":{\"drawable\":0,\"texture\":0},\"decals\":{\"drawable\":0,\"texture\":0},\"shirts\":{\"drawable\":0,\"texture\":0},\"helmets\":{\"drawable\":-1,\"texture\":0},\"glasses\":{\"drawable\":-1,\"texture\":0},\"earAccesories\":{\"drawable\":-1,\"texture\":0},\"watches\":{\"drawable\":-1,\"texture\":0},\"bracelets\":{\"drawable\":-1,\"texture\":0}}";
        private ClothesManager()
        {

        }

        [RemoteEvent("CLIENT:SERVER:GetClothes")]
        public static void GetClothes(Player player, byte index)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var clothes = CharacterDbManager.getClothes(character, index);
            string clothesJson;
            if (clothes != null)
                clothesJson = NAPI.Util.ToJson(clothes);
            else
                clothesJson = defaultClothesJSON;
            player.TriggerEvent("SERVER:CLIENT:ShowCostume", clothesJson);
            ClothesUpdate(player, clothesJson);

            character.Clothes[index] = clothes;
            character.currentOutfit = index;
        } 
        [Command("getoutfit")] //debug command
        private void outfit(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            character.SendChatMessage(character.currentOutfit.ToString());
        }
        [RemoteEvent("CLIENT:SERVER:BuyClothes")]
        private void BuyClothes(Player player, string ClothesJSON)
        {
            var clothes = NAPI.Util.FromJson<ClothesData>(ClothesJSON);
            var character = CharacterManager.PlayerCharacterPair[player];
            UpdateCharDataDBManager.UpdateCharacterClothes(character, clothes);
            character.currentOutfit = clothes.Id;
            ClothesUpdate(player, ClothesJSON);
        }
        [RemoteEvent("CLIENT:SERVER:ClothesUpdate")] //event not called anymore since moved clothes to clientside, method still useful above
        private static void ClothesUpdate(Player player, string json)
        {
            NAPI.Util.ConsoleOutput(json);
            var clothesData = NAPI.Util.FromJson<ClothesData>(json);
            var clothes = GetClothesComponents(clothesData);
            SetProps(player, clothesData);
            player.SetClothes(clothes);
        }
        private struct Prop
        {
            public int drawable;
            public int texture;
            public Prop(int drawable, int texture)
            {
                this.drawable = drawable;
                this.texture = texture;
            }
        }
        private static void SetProps(Player player, ClothesData clothesData)
        {
            Prop hat = new Prop(clothesData.Helmets.Drawable, clothesData.Helmets.Texture);
            Prop glasses = new Prop(clothesData.Glasses.Drawable, clothesData.Glasses.Texture);
            Prop ears = new Prop(clothesData.EarAccesories.Drawable, clothesData.EarAccesories.Texture);
            Prop watches = new Prop(clothesData.Watches.Drawable, clothesData.Watches.Texture);
            Prop bracelets = new Prop(clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture);

            if (clothesData.Helmets.Drawable == -1)
                NAPI.Player.ClearPlayerAccessory(player, 0);
            else
                NAPI.Player.SetPlayerAccessory(player, 0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture);

            if (clothesData.Glasses.Drawable == -1)
                NAPI.Player.ClearPlayerAccessory(player, 1);
            else
                NAPI.Player.SetPlayerAccessory(player, 1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture);

            if (clothesData.EarAccesories.Drawable == -1)
                NAPI.Player.ClearPlayerAccessory(player, 2);
            else
                NAPI.Player.SetPlayerAccessory(player, 2, clothesData.EarAccesories.Drawable, clothesData.EarAccesories.Texture);

            if (clothesData.Watches.Drawable == -1)
                NAPI.Player.ClearPlayerAccessory(player, 6);
            else
                NAPI.Player.SetPlayerAccessory(player, 6, clothesData.Watches.Drawable, clothesData.Watches.Texture);

            if (clothesData.Bracelets.Drawable == -1)
                NAPI.Player.ClearPlayerAccessory(player, 7);
            else
                NAPI.Player.SetPlayerAccessory(player, 7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture);

        }

        private static Dictionary<int, ComponentVariation> GetClothesComponents(ClothesData clothesData)
        {
            Dictionary<int, ComponentVariation> clothes = new Dictionary<int, ComponentVariation>();

            ComponentVariation masks = new ComponentVariation(clothesData.Mask.Drawable, clothesData.Mask.Texture);
            clothes.Add(1, masks);
            ComponentVariation torso = new ComponentVariation(clothesData.Torso.Drawable, clothesData.Torso.Texture);
            clothes.Add(3, torso);
            ComponentVariation legs = new ComponentVariation(clothesData.Legs.Drawable, clothesData.Legs.Texture);
            clothes.Add(4, legs);
            ComponentVariation bag = new ComponentVariation(clothesData.Bag.Drawable, clothesData.Bag.Texture);
            clothes.Add(5, bag);
            ComponentVariation shoes = new ComponentVariation(clothesData.Shoes.Drawable, clothesData.Shoes.Texture);
            clothes.Add(6, shoes);
            ComponentVariation accesories = new ComponentVariation(clothesData.Accessories.Drawable, clothesData.Accessories.Texture);
            clothes.Add(7, accesories);
            ComponentVariation undershirt = new ComponentVariation(clothesData.Undershirt.Drawable, clothesData.Undershirt.Texture);
            clothes.Add(8, undershirt);
            ComponentVariation armor = new ComponentVariation(clothesData.BodyArmor.Drawable, clothesData.BodyArmor.Texture);
            clothes.Add(9, armor);
            ComponentVariation decals = new ComponentVariation(clothesData.Decals.Drawable, clothesData.Decals.Texture);
            clothes.Add(10, decals);
            ComponentVariation shirts = new ComponentVariation(clothesData.Shirts.Drawable, clothesData.Shirts.Texture);
            clothes.Add(11, shirts);

            return clothes;
        }
    }
}
