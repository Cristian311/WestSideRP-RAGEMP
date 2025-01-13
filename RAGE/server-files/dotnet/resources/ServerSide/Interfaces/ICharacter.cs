using GTANetworkAPI;
using ServerSide.Characters.Customization.Clothes;
using ServerSide.Characters.Customization.Customize;
using ServerSide.Characters.Customization.Hair;
using ServerSide.Server.Interfaces;
using ServerSide.Server.Items;
using ServerSide.Server.Vehicles;
using System.Collections.Generic;
using System.Diagnostics;

namespace ServerSide.Characters
{
    public interface ICharacter
    {
        // Public Properties
        IAccount Account { get; }
        Player player { get; }
        string accountName { get; }
        int Id { get; set; }
        string Name { get; }
        int Age { get; }
        int Money { get; set; }
        bool Gender { get; set; }
        bool isCreated { get; set; }
        bool isCop { get; set; }
        CharacterData Face { get; set; }
        HairData Hair { get; set; }
        int currentOutfit { get; set; }
        Character currentCharacter { get; }
        int faction { get; set; }
        int factionRank { get; set; }

        // Collections
        List<ICharacter> Characters { get; }
        List<Item> Items { get; set; }
        List<WeaponHash> currentWeapons { get; set; }
        List<Decoration> Tattoos { get; set; }
        List<string> TattooNames { get; set; }
        List<IVehicle> PersonalVehicles { get; set; }
        List<IVehicle> AccessVehicles { get; set; }

        // GTANetworkAPI Related
        GTANetworkAPI.Vehicle CurrentDrivingVehicle { get; set; }

        // Miscellaneous
        Stopwatch ameStopwatch { get; set; }
        bool isInInterior { get; set; }
        Vector3 InteriorPosition { get; set; }
        ColShape exitColShape { get; set; }
        ColShape currentColshape { get; set; }

        // Static Properties
        static int nrOfCharacters { get; }

        // Id Manager
        IdManager<Item> ItemIdManager { get; }

        // MainPhone Property
        Phone MainPhone { get; set; }

        // Clothes Data
        ClothesData[] Clothes { get; set; }

        // Methods
        void StartAmeStopwatch();
        void GetCharacterInfo();
        void SendChatMessage(string message);
    }
}
