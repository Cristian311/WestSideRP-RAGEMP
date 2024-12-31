using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server.Shops
{
    internal class ShopData
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public SerializableVector3 Position { get; set; }
        public List<ShopItem> items { get; set; }
        public ShopData(string name, Vector3 position, List<ShopItem> items)
        {
            Name = name;
            Position = new SerializableVector3(position);
            this.items = items;
        }
    }
    public class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SerializableVector3(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
