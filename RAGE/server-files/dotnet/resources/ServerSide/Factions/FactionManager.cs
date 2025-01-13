using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Factions;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;

public class FactionManager : Script
{
    private static List<Faction> factions = new List<Faction>();
    private static Dictionary<Character, int> FactionInvitations = new Dictionary<Character, int>();
    private static int nextFactionId = 0;
    FactionManager()
    {
        LoadFactionsFromDatabase();
    }

    [RequiresAdmin]
    [Command("joinfaction")]
    private void JoinFactionCommand(Player player, int factionID)
    {
        if(factionID >= nextFactionId)
        {
            player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu exista acea factiune.");
            return;
        }
        var character = CharacterManager.PlayerCharacterPair[player];
        character.faction = factionID;
        character.factionRank = 7;
        var faction = GetFactionById(factionID);
        faction.Characters.Add(character);
        player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai intrat in factiunea {Colors.COLOR_CHAT_CRIMSON}{faction.Name}.");
    }
    private static Faction GetFactionById(int Id)
    {
        return factions.FirstOrDefault(f => f.Id == Id);
    }
    //[RequiresFactionRank]
    //[Command("finvite")]
    //private void FinviteCommand(Player player, string target)
    //{
    //    var character
    //}
    [RequiresFaction]
    [Command("fonline")]
    private void fonlineCommand(Player player)
    {
        var character = CharacterManager.PlayerCharacterPair[player];
        var faction = GetFactionById(character.faction);

        foreach(var member in faction.GetOnlineCharacters())
        {
            var account = AccountManager.PlayerAccountPairs[player];
            if(account.AdminDuty == false)
                player.SendChatMessage($"{Colors.COLOR_CHAT_WHISPER}{faction.RankNames[member.factionRank]} {member.Name}");
            else
                player.SendChatMessage($"{Colors.COLOR_CHAT_WHISPER}{faction.RankNames[member.factionRank]} {Colors.COLOR_CHAT_ADMIN}{account.Name}");

        }
    }
    [RequiresFaction]
    [Command("f", GreedyArg = true)]
    private void FactionChatCommand(Player player, string message)
    {
        var character = CharacterManager.PlayerCharacterPair[player];
        var faction = GetFactionById(character.faction);
        SendFactionMessage(character.faction, $"{Colors.COLOR_CHAT_INFO}(( {faction.RankNames[character.factionRank]} {character.Name}: {message} ))");
    }
    public static void SendFactionMessage(int factionId, string message)
    {
        var faction = GetFactionById(factionId);
        foreach (var factionMember in faction.Characters)
        {
            if (CharacterManager.loggedCharacters.Contains(factionMember))
            {
                factionMember.SendChatMessage(message);
            }
        }
    }
    [RequiresFaction]
    [RequiresFactionRank]
    [Command("ranks")]
    private void RanksCommand(Player player)
    {
        var character = CharacterManager.PlayerCharacterPair[player];
        var faction = GetFactionById(character.faction);

        for(int i = 1; i <= faction.RankNames.Count; i++)
        {
            player.SendChatMessage($"{i}. {faction.RankNames[i]}");
        }
    }
    [RequiresFaction]
    [RequiresFactionRank(5)]
    [Command("assignrank")]
    public static void AssignRankCommand(Player player, string CharName, int rank)
    {
        var playerCharacter = CharacterManager.PlayerCharacterPair[player];
        var character = CharacterManager.GetCharacterByName(player, CharName);
        
        if (character == null) return;
        var faction = GetFactionById(character.faction);

        if (faction.RankNames.ContainsKey(rank))
        {
            character.factionRank = rank;
            FactionManager.SendFactionMessage(playerCharacter.faction, $"{character.Name} has been assigned rank: {faction.RankNames[rank]}");
        }
        else
        {
            player.SendChatMessage("Invalid rank.");
        }
    }

    [RequiresAdmin]
    [Command("createfaction", GreedyArg = true)]
    private void CreateFactionCommand(Player player, string type, string name)
    {
        var newFaction = new Faction(nextFactionId, name, type);
        factions.Add(newFaction);
        nextFactionId++;
        SaveFactionToDatabase(newFaction); // Save to DB
        var account = AccountManager.PlayerAccountPairs[player];
        AdminManager.SendChatMessageToAdmins($"{Colors.COLOR_CHAT_ADMIN}{account.Name} a creat factiunea {newFaction.Type} ID {newFaction.Id} {newFaction.Name}");
    }

    // Load factions from the database
    public void LoadFactionsFromDatabase()
    {
        factions = LoadFactions(); // Load from DB
        if (factions.Count > 0)
        {
            nextFactionId = factions.Max(f => f.Id) + 1;
        }
    }
    private void SaveFactionToDatabase(Faction faction)
    {
        Console.WriteLine($"Faction saved: Id={faction.Id}, Name={faction.Name}");
    }

    private List<Faction> LoadFactions()
    {
        return new List<Faction>
        {
            new Faction(0, "FactionA", "Illegal"),
            new Faction(1, "FactionB", "Legal")
        };
    }
    [RequiresAdmin]
    [Command("factions")]
    public void DisplayFactions(Player player)
    {
        foreach (var faction in factions)
        {
            player.SendChatMessage($"Id: {faction.Id}, Name: {faction.Name}, Type: {faction.Type}");
        }
    }
}
