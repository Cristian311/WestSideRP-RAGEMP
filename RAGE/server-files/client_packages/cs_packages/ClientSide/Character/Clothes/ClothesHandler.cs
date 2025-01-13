using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character.Clothes
{
    internal class ClothesHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        private ClothesHandler()
        {
            Events.Add("SERVER:CLIENT:OpenClothesScreen", OpenClothesScreen);
            Events.Add("SERVER:CLIENT:CloseClothesScreen", CloseClothesScreen);
            Events.Add("CEF:CLIENT:CloseClothesScreen", CloseClothesScreen);
            Events.Add("CEF:CLIENT:UpdateClothes", UpdateClothes);
            Events.Add("CEF:CLIENT:SelectCostume", SelectClothes);
            Events.Add("CEF:CLIENT:BuyCostume", BuyClothes);
            Events.Add("SERVER:CLIENT:ShowCostume", ShowClothes);
            Events.Add("SERVER:CLIENT:UpdateDataMaxValues", UpdateDataMaxValues);
        }

        private void ShowClothes(object[] args)
        {

            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            cefBrowser.ExecuteJs($"UpdateClothes(\"{escapedJson}\");");
        }

        private void BuyClothes(object[] args)
        {
            string clothesJSON = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:BuyClothes", clothesJSON);
            CloseClothesScreen(null);
        }

        private void SelectClothes(object[] args)
        {
            byte index = byte.Parse(args[0].ToString());
            Events.CallRemote("CLIENT:SERVER:GetClothes", index);
        }
        private void UpdateClothes(object[] args)
        {
            string json = args[0].ToString();
            var clothesData = RAGE.Util.Json.Deserialize<ClothesData>(json);
            //Events.CallRemote("CLIENT:SERVER:ClothesUpdate", json);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(1, clothesData.Mask.Drawable, clothesData.Mask.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(3, clothesData.Torso.Drawable, clothesData.Torso.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(4, clothesData.Legs.Drawable, clothesData.Legs.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(5, clothesData.Bag.Drawable, clothesData.Bag.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(6, clothesData.Shoes.Drawable, clothesData.Shoes.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(7, clothesData.Accessories.Drawable, clothesData.Accessories.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(8, clothesData.Undershirt.Drawable, clothesData.Undershirt.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(9, clothesData.BodyArmor.Drawable, clothesData.BodyArmor.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(10, clothesData.Decals.Drawable, clothesData.Decals.Texture, 0);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(11, clothesData.Shirts.Drawable, clothesData.Shirts.Texture, 0);

            if (clothesData.Helmets.Drawable == -1)
                RAGE.Elements.Player.LocalPlayer.ClearProp(0);
            else
                RAGE.Elements.Player.LocalPlayer.SetPropIndex(0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture, true);

            if (clothesData.Glasses.Drawable == -1)
                RAGE.Elements.Player.LocalPlayer.ClearProp(1);
            else
                RAGE.Elements.Player.LocalPlayer.SetPropIndex(1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture, true);

            if (clothesData.EarAccesories.Drawable == -1)
                RAGE.Elements.Player.LocalPlayer.ClearProp(2);
            else
                RAGE.Elements.Player.LocalPlayer.SetPropIndex(2, clothesData.EarAccesories.Drawable, clothesData.EarAccesories.Texture, true);

            if(clothesData.Watches.Drawable == -1)
                RAGE.Elements.Player.LocalPlayer.ClearProp(6);
            else
                RAGE.Elements.Player.LocalPlayer.SetPropIndex(6, clothesData.Watches.Drawable, clothesData.Watches.Texture, true);

            if(clothesData.Bracelets.Drawable == -1)
                RAGE.Elements.Player.LocalPlayer.ClearProp(7);
            else
                RAGE.Elements.Player.LocalPlayer.SetPropIndex(7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture, true);
        }
        private void CloseClothesScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }
        private void UpdateDataMaxValues(object[] args)
        {
            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");

            cefBrowser.ExecuteJs($"updateMaxValuesFromJSON(\"{escapedJson}\");");
        }
        private void OpenClothesScreen(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/clothes/clothes.html");

            cefBrowser.Active = true;
            Cursor.Visible = true;

            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");

            int currentClothes = int.Parse(args[1].ToString());

            cefBrowser.ExecuteJs($"UpdateClothes(\"{escapedJson}\");");
            //cefBrowser.ExecuteJs($"selectItem(\"{currentClothes}\");");
        }
    }
}
