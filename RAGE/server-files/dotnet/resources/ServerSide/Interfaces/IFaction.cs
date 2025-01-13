using ServerSide.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Interfaces
{
    public interface IFaction
    {
        int Id { get; }
        string Name { get; }
        string Type { get; }
        List<string> Characters { get; set; }
        Dictionary<int, string> RankNames { get; set; }

        List<Character> GetOnlineCharacters();
    }
}
