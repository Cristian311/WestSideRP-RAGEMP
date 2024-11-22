using GTANetworkAPI;
using ServerSide.Characters.Clothes;
using ServerSide.Hair;
using ServerSide.Interfaces;
using ServerSide.Items;
using System;
using System.Collections.Generic;
using System.Text;

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
        public bool gender { get; set; }
        public bool isCreated { get; set; } = false;
        public bool isCop { get; set; } = false;
        public CharacterData Face { get; set; }
        public HairData Hair { get; set; }
        public List<ClothesData> Clothes { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        public PlayerIdManager ItemIdManager = new PlayerIdManager();
        public Character(Player player, IAccount account, string name, int age, int money)
        {
            Account = account;
            Id = nrOfCharacters++;
            Name = name; 
            Age = age;
            Money = money;
        }
        public void getCharacterInfo()
        {
            NAPI.Util.ConsoleOutput($"Account: {Account.Name} ID: {Id}");
            NAPI.Util.ConsoleOutput($"Name: {Name} Age: {Age} Money: {Money}, isCreated: {isCreated} Gender{gender}");
        }
    }
}
