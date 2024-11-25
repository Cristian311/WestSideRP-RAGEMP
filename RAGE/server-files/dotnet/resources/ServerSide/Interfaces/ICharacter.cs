using GTANetworkAPI;
using ServerSide.Characters.Clothes;
using ServerSide.Characters.Customize;
using ServerSide.Hair;
using ServerSide.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Interfaces
{
    public interface ICharacter
    {
        IAccount Account { get; }
        int Id { get; }
        string Name { get; }
        int Age { get; }
        int Money { get; }
        public bool isCreated { get; set; }
        CharacterData Face { get; set; }
        HairData Hair { get; set; }
        ClothesData[] Clothes { get; set; }
        List<Item> Items { get; set; }
        List<Decoration> Tattoos { get; set; }
        int currentOutfit { get; set; }
        bool Gender {  get; set; }
    }
}
