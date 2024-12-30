using GTANetworkAPI;
using ServerSide.Characters.Customization.Clothes;
using ServerSide.Characters.Customization.Customize;
using ServerSide.Characters.Customization.Hair;
using ServerSide.Server.Interfaces;
using ServerSide.Server.Items;

using System.Collections.Generic;

using Vehicle = ServerSide.Server.Vehicles.Vehicle;

namespace ServerSide.Characters
{
    public class Character : ICharacter
    {
        public IAccount Account { get; }
        public Player player { get; }
        public string accountName { get; }
        public int Id { get; set; }
        public static int nrOfCharacters = 0; 
        public string Name { get; }
        public int Age { get; }   
        public int Money { get; }
        public List<ICharacter> Characters { get; }
        public Character currentCharacter { get; }
        public int currentOutfit { get; set; }
        public bool Gender { get; set; }
        public bool isCreated { get; set; } = false;
        public bool isCop { get; set; } = false;
        public CharacterData Face { get; set; }
        public HairData Hair { get; set; }
        public ClothesData[] Clothes { get; set; } = new ClothesData[20];
        public List<Item> Items { get; set; } = new List<Item>();
        public Phone MainPhone { get; set; }
        public List<WeaponHash> currentWeapons {  get; set; } = new List<WeaponHash>();
        public List<Decoration> Tattoos { get; set; } 
        public IdManager<Item> ItemIdManager = new IdManager<Item>();
        public ColShape currentColshape { get; set; }
        public List<string> TattooNames { get; set; } = new List<string>();
        public List<Vehicle> PersonalVehicles { get; set; } = new List<Vehicle>();
        public GTANetworkAPI.Vehicle CurrentDrivingVehicle { get; set; }
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
