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
    public class Character : ICharacter
    {
        // Public Properties
        public IAccount Account { get; }
        public Player player { get; }
        public string accountName { get; }
        public int Id { get; set; }
        public string Name { get; }
        public int Age { get; }
        public int Money { get; set; }
        public bool Gender { get; set; }
        public bool isCreated { get; set; } = false;
        public bool isCop { get; set; } = false;
        public CharacterData Face { get; set; }
        public HairData Hair { get; set; }
        public int currentOutfit { get; set; }
        public Character currentCharacter { get; }

        // Collections
        public List<ICharacter> Characters { get; }
        public List<Item> Items { get; set; } = new List<Item>();
        public List<WeaponHash> currentWeapons { get; set; } = new List<WeaponHash>();
        public List<Decoration> Tattoos { get; set; }
        public List<string> TattooNames { get; set; } = new List<string>();
        public List<IVehicle> PersonalVehicles { get; set; } = new List<IVehicle>();
        public List<IVehicle> AccessVehicles { get; set; } = new List<IVehicle>();

        // GTANetworkAPI Related
        public GTANetworkAPI.Vehicle CurrentDrivingVehicle { get; set; }

        // Miscellaneous
        public Stopwatch ameStopwatch { get; set; }
        public bool isInInterior { get; set; }
        public Vector3 InteriorPosition { get; set; }
        public ColShape exitColShape { get; set; }
        public ColShape currentColshape { get; set; }

        // Static Properties
        public static int nrOfCharacters = 0;

        // Id Manager
        public IdManager<Item> ItemIdManager { get; set; } = new IdManager<Item>();

        // MainPhone Property
        public Phone MainPhone { get; set; }

        // Clothes Data
        public ClothesData[] Clothes { get; set; } = new ClothesData[20];
        public int faction { get; set; } = -1;
        public int factionRank { get; set; } = 0;

        // Constructor
        public Character(Player player, IAccount account, string name, int age, int money)
        {
            this.player = player;
            Account = account;
            Id = nrOfCharacters++;
            Name = name;
            Age = age;
            Money = money;
            Items = new List<Item>();
            Tattoos = new List<Decoration>();
        }

        // Methods
        public void StartAmeStopwatch()
        {
            ameStopwatch.Start();
        }

        public void GetCharacterInfo()
        {
            NAPI.Util.ConsoleOutput($"Account: {Account.Name} ID: {Id}");
            NAPI.Util.ConsoleOutput($"Name: {Name} Age: {Age} Money: {Money}, isCreated: {isCreated} Gender{Gender}");
        }

        public void SendChatMessage(string message)
        {
            player.SendChatMessage(message);
        }
    }
}
