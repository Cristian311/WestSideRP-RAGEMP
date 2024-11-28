using Newtonsoft.Json;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.DB.CharacterDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Items
{
    public class Item
    {
        private static int ItemsCounter = 0;
        public int Id {  get; }
        public int localID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string UsedFor { get; set; }
        public int Amount { get; set; }
        public bool Countable { get; }
        public int PhoneNumber { get; set; }
        public int currentCharacterId { get; } = 0;
        [JsonIgnore]
        public Character currentCharacter { get; set; }
        public Item(Character character, string Name, string Type, int Amount, bool Countable, string UsedFor = "")
        {
            Id = ItemsCounter;
            ItemsCounter++;

            if (character != null)
            {
                currentCharacterId = character.Id;
                localID = character.ItemIdManager.AssignId();
                currentCharacter = character;
            }
            else
                currentCharacter = ItemManager.GetCharacterById(currentCharacterId);

            this.Name = Name;
            this.Type = Type.ToLower();
            this.Amount = Amount;
            this.Countable = Countable;
            this.UsedFor = UsedFor;
        }
    }
}
