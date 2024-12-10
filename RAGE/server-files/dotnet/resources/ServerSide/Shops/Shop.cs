using GTANetworkAPI;
using ServerSide.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Shops
{
    internal class Shop
    {
        public string Name { get; set; }
        public ColShape colShape {  get; set; }
        public List<ShopItem> items { get; set; }
        public TextLabel textLabel { get; set; }
        public Shop(string Name, ColShape colShape, TextLabel textLabel)
        {
            this.Name = Name;
            this.textLabel = textLabel;
            this.colShape = colShape;
        }
    }
}
