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
        public int Id { get; }
        public static int nrOfCharacters = 0; 
        public string Name { get; }
        public int Age { get; }   
        public int Money { get; }
        public List<ICharacter> Characters { get; }
        public Character currentCharacter { get; }
        public Character(IAccount account, string name, int age, int money)
        {
            Account = account;
            Id = nrOfCharacters++;
            Name = name; 
            Age = age;
            Money = money;
        }
    }
}
