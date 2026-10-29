using GTANetworkAPI;
using System.Collections.Generic;

namespace ServerSide.Server.Shops
{
    internal class Shop
    {
        public string Name { get; set; }
        public ColShape colShape { get; set; }
        public List<ShopItem> items { get; set; }
        public TextLabel textLabel { get; set; }
        public Vector3 Position { get; set; }
        public Shop(string Name, ColShape colShape, TextLabel textLabel, Vector3 position)
        {
            this.Name = Name;
            this.textLabel = textLabel;
            this.colShape = colShape;
            Position = position;
        }
    }
}
