using GTANetworkAPI;

namespace ServerSide.Interfaces
{
    internal interface IHouse
    {
        int Id { get; set; }
        string Name { get; }
        string Owner { get; set; }
        string OwnerType { get; set; }
        bool LockStatus { get; set; }
        int Value { get; set; }
        int InteriorCustomObjects { get; set; }
        Vector3 Position { get; set; }
        Vector3 InteriorPosition { get; set; }
        ColShape ColShape { get; set; }
        ColShape ExitColShape { get; set; }
        uint Dimension { get; set; }

    }
}
