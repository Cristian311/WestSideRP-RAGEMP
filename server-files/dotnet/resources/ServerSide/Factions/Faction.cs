using ServerSide.Characters;
using ServerSide.Interfaces;
using ServerSide.Server.Logger;
using System.Collections.Generic;

namespace ServerSide.Factions
{
    public class Faction : IFaction
    {
        public int Id { get; }
        public string Name { get; }
        public string Type { get; }
        public List<string> Characters { get; set; } = new List<string>();
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
        public bool HasPermission(Character character, int requiredRank)
        {
            return character.factionRank >= requiredRank;
        }

        public List<Character> GetOnlineCharacters()
        {
            List<Character> characters = new List<Character>();
            foreach (var character in CharacterManager.loggedCharacters)
            {
                if (character.faction == Id)
                    characters.Add(character);
            }
            return characters;
        }
        public void SendFactionMessage(string message)
        {
            ServerLogger.Log($"Faction {Name}: " + message);
            foreach (var factionMember in GetOnlineCharacters())
            {
                factionMember.SendChatMessage(message);
            }
        }
    }
}
