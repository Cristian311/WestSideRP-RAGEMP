using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Color = GTANetworkAPI.Color;

namespace ServerSide.Shops
{
    internal class ShopManager : Script
    {
        public static List<Shop> shops = new List<Shop>();
        public static List<ColShape> shopColshapes = new List<ColShape>();
        [Command("createshop")]
        private void CreateShopCommand(Player player, string Name)
        {
            ColShape colShape = NAPI.ColShape.CreateCylinderColShape(player.Position, 1f, 1f, NAPI.GlobalDimension);
            TextLabel textLabel = NAPI.TextLabel.CreateTextLabel("Apasa Y sa deschizi shopul", player.Position, 5f, 1f, 0, new Color(0, 250, 0), true, NAPI.GlobalDimension);
            Shop shop = new Shop(Name, colShape, textLabel);
            shops.Add(shop);
            shopColshapes.Add(colShape);
        }
    }
}
