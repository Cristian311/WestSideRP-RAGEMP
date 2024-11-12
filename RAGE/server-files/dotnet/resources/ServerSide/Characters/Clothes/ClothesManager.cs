using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters.Clothes
{
    internal class ClothesManager : Script
    {
        [RemoteEvent("CLIENT:SERVER:ClothesUpdate")]
        private void ClothesUpdate(Player player, string json)
        {
            var clothesData = NAPI.Util.FromJson<ClothesData>(json);
            var clothes = getClothes(clothesData);
            player.SetClothes(clothes);
            NAPI.Util.ConsoleOutput(json);
        }
        private Dictionary<int, ComponentVariation> getClothes(ClothesData clothesData)
        {
            Dictionary<int, ComponentVariation> clothes = new Dictionary<int, ComponentVariation>();

            //ID 1 MASKS
            ComponentVariation masks = new ComponentVariation(clothesData.Mask.Drawable, clothesData.Mask.Texture);
            clothes.Add(1, masks);
            //ID 3 TORSOS
            ComponentVariation torso = new ComponentVariation(clothesData.Torso.Drawable, clothesData.Torso.Texture);
            clothes.Add(3, torso);
            //ID 4 LEGS
            ComponentVariation legs = new ComponentVariation(clothesData.Legs.Drawable, clothesData.Legs.Texture);
            clothes.Add(4, legs);
            //ID 5 BAGS AND PARACHUTES
            ComponentVariation bag = new ComponentVariation(clothesData.Bag.Drawable, clothesData.Bag.Texture);
            clothes.Add(5, bag);
            //ID 6 SHOES
            ComponentVariation shoes = new ComponentVariation(clothesData.Shoes.Drawable, clothesData.Shoes.Texture);
            clothes.Add(6, shoes);
            //ID 7 ACCESORIES
            ComponentVariation accesories = new ComponentVariation(clothesData.Accessories.Drawable, clothesData.Accessories.Texture);
            clothes.Add(7, accesories);
            //ID 8 UNDERSHIRTS
            ComponentVariation undershirt = new ComponentVariation(clothesData.Undershirt.Drawable, clothesData.Undershirt.Texture);
            clothes.Add(8, undershirt);
            //ID 9 BODY ARMOR
            ComponentVariation armor = new ComponentVariation(clothesData.BodyArmor.Drawable, clothesData.BodyArmor.Texture);
            clothes.Add(9, armor);
            //ID 10 DECALS
            ComponentVariation decals = new ComponentVariation(clothesData.Decals.Drawable, clothesData.Decals.Texture);
            clothes.Add(10, decals);
            //ID 11 TOPS
            ComponentVariation tops = new ComponentVariation(clothesData.Tops.Drawable, clothesData.Tops.Texture);
            clothes.Add(11, tops);

            return clothes;
        }
    }
}
