using Newtonsoft.Json;
using ServerSide.Characters;
using System.Linq;

namespace ServerSide.Server.Items
{
    public class Item
    {
        private static int ItemsCounter = 0;
        public int Id { get; }
        public int LocalId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public bool isStackable { get; }
        public int PhoneNumber { get; set; }
        public int CurrentCharacterId { get; } = 0;
        [JsonIgnore]
        public Character CurrentCharacter { get; set; }
        public Item(Character character, string Name, string Type, int Amount, bool Countable)
        {
            Id = ItemsCounter;
            ItemsCounter++;

            if (character != null)
            {
                CurrentCharacterId = character.Id;
                LocalId = character.ItemIdManager.AssignNewId(this);
                CurrentCharacter = character;
            }
            else
                CurrentCharacter = ItemManager.GetCharacterById(CurrentCharacterId);

            this.Name = Name;
            this.Type = Type.ToLower();
            this.Amount = Amount;
            this.isStackable = Countable;

            if (Type.ToLower() == "phone")
            {
                PhoneNumber = character?.Items?.FirstOrDefault(i => i.Type == "phone" && i.PhoneNumber != 0)?.PhoneNumber ?? 0;
            }
        }
    }
}
