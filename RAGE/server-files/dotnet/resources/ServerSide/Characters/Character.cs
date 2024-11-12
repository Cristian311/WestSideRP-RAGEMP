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
        public bool gender { get; set; }
        public bool isCreated { get; set; } = false;
        public string CharacterFaceDetailsJSON { get; set; } = "{\"shape\":{\"first\":\"0\",\"second\":\"0\",\"mix\":\"0\"},\"skin\":{\"first\":\"0\",\"second\":\"0\",\"mix\":\"0\"},\"nose\":{\"width\":\"0\",\"height\":\"0\",\"length\":\"0\",\"bridge\":\"0\",\"tip\":\"0\",\"bridgeShift\":\"0\"},\"brow\":{\"height\":\"0\",\"width\":\"0\"},\"cheeks\":{\"height\":\"0\",\"width\":\"0\",\"cheeksWidth\":\"0\"},\"jaw\":{\"width\":\"0\"},\"chin\":{\"lenght\":\"0\",\"position\":\"0\",\"width\":\"0\",\"shape\":\"0\"},\"neck\":{\"width\":\"0\"},\"lips\":\"0\",\"skinColor\":\"0\",\"eye\":{\"size\":\"0\",\"color\":\"0\"}}\r\n";
        public string HairJSON { get; set; } = "{\"hair\":\"0\",\"facialHair\":{\"value\":\"0\",\"color\":\"0\",\"secondaryColor\":\"0\",\"opacity\":1},\"eyebrows\":{\"value\":\"0\",\"color\":\"0\",\"secondaryColor\":\"0\",\"opacity\":\"1\"},\"bodyHair\":{\"value\":\"0\",\"color\":\"0\",\"secondaryColor\":\"0\",\"opacity\":\"1\"},\"hairTattoo\":{\"value\":\"0\",\"color\":\"0\",\"secondaryColor\":\"0\",\"opacity\":\"1\"}}\r\n";
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
            NAPI.Util.ConsoleOutput($"Name: {Name} Age: {Age} Money: {Money}, isCreated: {isCreated} Gender{gender}");
        }
    }
}
