using GTANetworkAPI;
using ServerSide.Interfaces;

namespace ServerSide.Server.Houses
{
    internal class House : IHouse
    {
        public int Id { get; set; }

        public string Name { get; }

        public string Owner { get; set; }
        public string OwnerType { get; set; }
        public int Value { get; set; }
        public int InteriorCustomObjects { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 InteriorPosition { get; set; }
        public ColShape ColShape { get; set; }
        public ColShape ExitColShape { get; set; }
        public uint Dimension { get; set; }
        public bool LockStatus { get; set; } = false;

        public House(string Name, string Owner, string OwnerType, Vector3 position, ColShape colShape)
        {
            this.Name = Name;
            this.Owner = Owner;
            this.OwnerType = OwnerType;
            this.Position = position;
            this.ColShape = colShape;
        }
    }
}
