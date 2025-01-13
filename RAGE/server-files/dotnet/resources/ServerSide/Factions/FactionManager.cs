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
    public static Dictionary<Character, int> FactionInvitations = new Dictionary<Character, int>();
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
            player.SendChatMessage($"{Colors.ERROR}Nu exista acea factiune.");
            return;
        }
        var character = CharacterManager.PlayerCharacterPair[player];
        character.faction = factionID;
        character.factionRank = 7;
        var faction = GetFactionById(factionID);
        faction.Characters.Add(character);
        player.SendChatMessage($"{Colors.INFO}Ai intrat in factiunea {Colors.CRIMSON}{faction.Name}.");
    }
    private static Faction GetFactionById(int Id)
    {
        return factions.FirstOrDefault(f => f.Id == Id);
    }
    [RequiresFaction]
    [RequiresFactionRank]
    [Command("finvite")]
    private void FinviteCommand(Player player, string target)
    {
        var playerCharacter = CharacterManager.PlayerCharacterPair[player];
        var character = CharacterManager.GetCharacterByName(player, target);
        if (character == null) return;

        var faction = GetFactionById(playerCharacter.faction);
        if(character.faction == -1)
        {
            playerCharacter.SendErrorMessage("Acel player este deja intr-o factiune.");
            return;
        }
        if (FactionInvitations.ContainsKey(character))
        {
            character.SendErrorMessage("Acest character este deja invitat intr-o factiune.");
            var invitedFaction = GetFactionById(FactionInvitations[character]);
            playerCharacter.SendErrorMessage($"{character.Name} incearca sa te invite in factiunea {faction.Name}, dar tu esti deja invitat in factiunea {invitedFaction.Name}. Foloseste /faccept sau /fdecline ca sa intrii in factiunea {invitedFaction.Name}");
            return;
        }
        FactionInvitations.Add(character, faction.Id);
        character.SendInfoMessage($"Ai fost invitat in factiunea {faction.Name}. Foloseste /faccept ca sa accepti sau /fdecline ca sa refuzi oferta.");
        playerCharacter.SendInfoMessage($"Ai invitat pe {character.Name} in factiunea {faction.Name}");
    }
    [RequiresFactionInvitation]
    [Command("faccept")]
    private void facceptCommand(Player player, string confirm)
    {
        var character = CharacterManager.PlayerCharacterPair[player];

        if(confirm.Equals("confirm"))
        {
            character.faction = FactionInvitations[character];
            character.factionRank = 1;

            var faction = GetFactionById(character.faction);
            character.SendChatMessage($"{Colors.GREEN}Ai intrat in factiunea {faction.Name}!");
            faction.SendFactionMessage($"{Colors.INFO}{character.Name} a intrat in factiune!");

            FactionInvitations.Remove(character);
        }
        else
        {
            character.SendChatMessage($"{Colors.BEIGE}Foloseste /faccept confirm ca sa intrii in factiune.");
        }
    }

    [RequiresFactionInvitation]
    [Command("fdecline")]
    private void fdecline(Player player, string confirm)
    {
        var character = CharacterManager.PlayerCharacterPair[player];
        if (confirm.Equals("confirm"))
        {
            var faction = GetFactionById(character.faction);
            character.SendChatMessage($"{Colors.GREEN}Ai refuzat sa intrii in factiunea {faction.Name}!");

            FactionInvitations.Remove(character);
        }
        else
        {
            character.SendChatMessage($"{Colors.BEIGE}Foloseste /fdecline confirm ca sa refuzi invitatia in factiune.");
        }
    }
    [RequiresAdmin]
    [RequiresFaction]
    [Command("fa", GreedyArg = true)]
    private void FactionAdminMessage(Player player, string message)
    {
        var character = CharacterManager.PlayerCharacterPair[player];
        var account = AccountManager.PlayerAccountPairs[player];
        var faction = GetFactionById(character.faction);
        if (account.AdminDuty == true)
            SendFactionMessage(character.faction, $"{Colors.INFO}(( {faction.RankNames[character.factionRank]} {Colors.DARK_RED}{account.Name}{Colors.INFO}: {message} ))");
        else
            character.SendErrorMessage("Trebuie sa fii on-duty sa folosesti aceasta comanda, foloseste /aduty");
    }
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
                player.SendChatMessage($"{Colors.WHISPER}{faction.RankNames[member.factionRank]} {member.Name}");
            else
                player.SendChatMessage($"{Colors.WHISPER}{faction.RankNames[member.factionRank]} {Colors.ADMIN}{account.Name}");
        }
    }
    [RequiresFaction]
    [Command("f", GreedyArg = true)]
    private void FactionChatCommand(Player player, string message)
    {
        var character = CharacterManager.PlayerCharacterPair[player];
        var faction = GetFactionById(character.faction);
        SendFactionMessage(character.faction, $"{Colors.INFO}(( {faction.RankNames[character.factionRank]} {character.Name}: {message} ))");
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
        AdminManager.SendChatMessageToAdmins($"{Colors.ADMIN}{account.Name} a creat factiunea {newFaction.Type} ID {newFaction.Id} {newFaction.Name}");
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
