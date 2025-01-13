using ServerSide.Characters;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Factions
{
    public class Faction : IFaction
    {
        public int Id { get; }
        public string Name { get; }
        public string Type { get; }
        public List<ICharacter> Characters { get; set; } = new List<ICharacter>();
        public Dictionary<int, string> RankNames { get; set; } 

        public Faction(int Id, string Name, string Type)
        {
            this.Id = Id;
            this.Name = Name;
            this.Type = Type;
        }

        public List<Character> GetOnlineCharacters()
        {
            List<Character> characters = new List<Character>();
            foreach (var character in CharacterManager.loggedCharacters)
            {
                if(character.faction == Id)
                    characters.Add(character);
            }
            return characters;
        }
    }
}
