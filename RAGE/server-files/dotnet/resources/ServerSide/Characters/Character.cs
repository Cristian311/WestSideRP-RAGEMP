using GTANetworkAPI;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters
{
    public class Character : ICharacter
    {

        public IAccount Account { get; }
        public string accountName { get; }
        public int Id { get; set; }
        public static int nrOfCharacters = 0; 
        public string Name { get; }
        public int Age { get; }   
        public int Money { get; }
        public List<ICharacter> Characters { get; }
        public Character currentCharacter { get; }
        public bool isCreated { get; set; } = false;
        public Character(IAccount account, string name, int age, int money)
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
            NAPI.Util.ConsoleOutput($"Name: {Name} Age: {Age} Money: {Money}, isCreated: {isCreated}");
        }
    }
}
