using ServerSide.Characters;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

            RankNames = new Dictionary<int, string>
            {
                { 1, "Outsider" },
                { 2, "Associate" },
                { 3, "Insider" },
                { 4, "Soldier" },
                { 5, "Sergeant" },
                { 6, "Brigadier" },
                { 7, "Authority" }
            };
        }
        public bool HasPermission(ICharacter character, int requiredRank)
        {
            return character.factionRank >= requiredRank;
        }

        public string GetFactionInfoJson()
        {
            var factionInfo = new
            {
                Id = this.Id,
                Name = this.Name,
                Type = this.Type,
                Members = Characters.Select(c => new { c.Name, Rank = RankNames[c.factionRank] })
            };

            return System.Text.Json.JsonSerializer.Serialize(factionInfo);
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
        public void SendFactionMessage(string message)
        {
            foreach (var factionMember in GetOnlineCharacters())
            {
                factionMember.SendChatMessage(message);
            }
        }
    }
}
