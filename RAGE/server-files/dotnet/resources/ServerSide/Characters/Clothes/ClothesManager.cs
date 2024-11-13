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
            NAPI.Util.ConsoleOutput(json);
            var clothesData = NAPI.Util.FromJson<ClothesData>(json);
            var clothes = getClothes(clothesData);
            setProps(player, clothesData);
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
        private void setProps(Player player, ClothesData clothesData)
        {
            Prop hat = new Prop(clothesData.Helmets.Drawable, clothesData.Helmets.Texture);
            Prop glasses = new Prop(clothesData.Glasses.Drawable, clothesData.Glasses.Texture);
            Prop ears = new Prop(clothesData.EarAccesories.Drawable, clothesData.EarAccesories.Texture);
            Prop watches = new Prop(clothesData.Watches.Drawable, clothesData.Watches.Texture);
            Prop bracelets = new Prop(clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture);

            NAPI.Player.SetPlayerAccessory(player, 0, hat.drawable, hat.texture);
            NAPI.Player.SetPlayerAccessory(player, 1, glasses.drawable, glasses.texture);
            NAPI.Player.SetPlayerAccessory(player, 2, ears.drawable, ears.texture);
            NAPI.Player.SetPlayerAccessory(player, 6, watches.drawable, watches.texture);
            NAPI.Player.SetPlayerAccessory(player, 7, bracelets.drawable, bracelets.texture);
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
            ComponentVariation shirts = new ComponentVariation(clothesData.Shirts.Drawable, clothesData.Shirts.Texture);
            clothes.Add(11, shirts);

            return clothes;
        }
    }
}
