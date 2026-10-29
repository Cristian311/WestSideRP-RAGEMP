using GTANetworkAPI;

namespace ServerSide.Server.Blips
{
    internal class BlipData
    {
        public int Id { get; set; }
        public uint Sprite { get; set; }
        public Vector3 Position { get; set; }
        public float Scale { get; set; }
        public byte Color { get; set; }
        public string Name { get; set; }
        public BlipData(string Name, Vector3 Position, float Scale, byte Color, uint Sprite)
        {
            this.Color = Color;
            this.Name = Name;
            this.Position = Position;
            this.Scale = Scale;
            this.Sprite = Sprite;
        }
    }
}
