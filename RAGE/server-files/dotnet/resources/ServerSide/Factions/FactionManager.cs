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
            player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu exista acea factiune");
            return;
        }
        var character = CharacterManager.PlayerCharacterPair[player];
        character.faction = factionID;
        var faction = GetFactionById(factionID);
        faction.Characters.Add(character);
        player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai intrat in factiunea {Colors.COLOR_CHAT_CRIMSON}{faction.Name}");
    }
    private static Faction GetFactionById(int Id)
    {
        return factions.FirstOrDefault(f => f.Id == Id);
    }

    [Command("f", Alias = "faction", GreedyArg = true)]
    private void FactionChatCommand(Player player, string message)
    {
        var character = CharacterManager.PlayerCharacterPair[player];
        if(character.faction == -1)
        {
            player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu esti intr-o factiune!");
            return;
        }
        var faction = GetFactionById(character.faction);
        foreach (var factionMember in faction.Characters)
        {
            if (CharacterManager.loggedCharacters.Contains(factionMember))
            {
                factionMember.SendChatMessage($"{Colors.COLOR_CHAT_INFO}(( {character.Name}: {message} ))");
            }
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
        AdminManager.SendChatMessageToAdmins($"{Colors.COLOR_CHAT_ADMIN}{account.Name} a creat factiunea ID {newFaction.Id} {newFaction.Name}");
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

    // Example placeholder for saving a faction to the database
    private void SaveFactionToDatabase(Faction faction)
    {
        // Add database save logic here
        Console.WriteLine($"Faction saved: Id={faction.Id}, Name={faction.Name}");
    }

    // Example placeholder for loading factions from the database
    private List<Faction> LoadFactions()
    {
        // Replace with actual DB loading logic
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
