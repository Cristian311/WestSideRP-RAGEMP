
using Google.Protobuf.WellKnownTypes;
using GTANetworkAPI;
using GTANetworkMethods;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Server.Commands;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.DB.CharacterDB;
using ServerSide.Server.Items.Util;
using ServerSide.Server.Items.Weapons;
using ServerSide.Server.Logger;
using ServerSide.Util;
using ServerSide.Util.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Server.Items
{
    internal class ItemManager : Script
    {
        internal class ItemData
        {
            public int id;
            public int amount;
            public string name;
            public string type;

            public ItemData(int ID, string Name, string Type, int Amount)
            {
                id = ID;
                name = Name;
                type = Type;
                amount = Amount;
            }
        }
        private ItemManager()
        {
            WeaponHashMapper.LoadWeapons("json\\weapon\\weapon_names.json");
        }

        [ServerEvent(Event.PlayerDisconnected)] // save items in db on disconnect
        private void SaveCharItems(Player player, DisconnectionType type, string reason)
        {
            if (CharacterManager.PlayerCharacterPair.ContainsKey(player) == false)
                return;
            var character = CharacterManager.PlayerCharacterPair[player];
            WeaponManager.UnequipCommand(player);
            ItemDbManager.UpdateCharacterItems(character);
        }

        

        [Command("inv")]
        private void InvCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.Items == null)
            {
                player.SendChatMessage($"{Colors.ERROR}Nu ai nici un item.");
                return;
            }
            //character.Items = FixItemsOrder(character.Items);

            List<ItemData> itemDataList = character.Items
                .Select(item => new ItemData(item.LocalId, item.Name, item.Type, item.Amount))
                .ToList();

            string json = NAPI.Util.ToJson(itemDataList);
            player.TriggerEvent("SERVER:CLIENT:OpenInventory", json);
        }

        [Command("invex")]
        private void InvexCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.Items == null)
            {
                player.SendChatMessage($"{Colors.ERROR}Nu ai nici un item.");
                return;
            }
            //
            //character.Items = FixItemsOrder(character.Items); // fix items indexes in inventory

            player.SendChatMessage($"{Colors.ITEMS}Itemele detinute in prezent:");
            foreach (var item in character.Items) // print items
            {
                if (item.Type == "phone")
                    player.SendChatMessage($"{Colors.ITEMS}[{item.LocalId}] (Phone) - {item.Name} - {item.PhoneNumber}");
                else
                    player.SendChatMessage($"{Colors.ITEMS}[{item.LocalId}] ({item.Type}) - {item.Name} - {item.Amount} units.");
            }
        }

        [Command("createitem")]
        private void CreateItemCommand(Player player, string Name, string Type, int Ammount)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            if (character.Items == null)
            {
                character.Items = new List<Item>();
            }

            bool isCountable = GetCountable(Type);

            if (!isCountable)
            {
                // For non-countable items, create individual instances
                for (int i = 1; i <= Ammount; i++)
                {
                    var newItem = new Item(character, Name, Type, 1, false);
                    newItem.LocalId = character.ItemIdManager.AssignNewId(newItem);
                    character.Items.Add(newItem);
                }
            }
            else
            {
                // For countable items, create a single item
                var newItem = new Item(character, Name, Type, Ammount, true);
                newItem.LocalId = character.ItemIdManager.AssignNewId(newItem);
                character.Items.Add(newItem);
            }
            player.SendChatMessage($"{Colors.ADMIN}Created item {Name} Type {Type} {Ammount} units.");
            ItemDbManager.UpdateCharacterItems(character);
        }

        

        [RequiresAdmin]
        [Command("createdrug")]
        private void CreateDrugCommand(Player player, string Name, int Ammount)
        {
            var character = CharacterManager.GetCharacter(player);
            var accountName = AccountManager.GetAccount(player).Name;

            Name = StringFormatter.CapitalizeFirstLetter(Name);


            character.Items.Add(new Item(character, Name, "drug", Ammount, true));
            ItemDbManager.UpdateCharacterItems(character);

            string adminMessage = $"{Colors.ADMIN}{accountName} a creat drogul {Name} ({Ammount} units).";
            AdminManager.SendChatMessageToAdmins(adminMessage);
            ServerLogger.Log(adminMessage);
        }

        

        


        [Command("giveitem")]
        public void GiveItemCommand(Player player, string target, int index, int ammount = 1)
        {
            var targetCharacter = CharacterManager.GetCharacterNearby(player, target);

            if (targetCharacter != null) // Proceed only if exactly one character is found
            {
                var character = CharacterManager.PlayerCharacterPair[player];

                var item = GetItemByIndex(character.Items, index);
                if (item.Amount < ammount)
                {
                    player.SendChatMessage($"{Colors.ERROR}N-ai {ammount} {item.Name}.");
                    return;
                }

                item.Amount -= ammount;
                if (item.Amount <= 0)
                {
                    character.Items.Remove(item);
                    character.ItemIdManager.ReclaimId(item.LocalId);
                }

                var receivedItem = new Item(targetCharacter, item.Name, item.Type, ammount, item.isStackable);
                targetCharacter.Items.Add(receivedItem);
                player.SendChatMessage($"{Colors.INFO}I-ai dat {Colors.DARK_BLUE}{ammount} {item.Name} {Colors.INFO}lui {targetCharacter.Name}.");
                targetCharacter.SendChatMessage($"{Colors.INFO}Ai primit {Colors.DARK_BLUE}{ammount} {item.Name} de la {Colors.INFO}{character.Name}.");
            }
            else
            {
                // If no valid character is found, we already printed the characters in GetCharacterNearby
                player.SendChatMessage($"{Colors.ERROR}Nu s-a gasit un caracter valid sau sunt prea multe persoane in jur cu acelasi nume.");
            }
        }
        [Command("showitem")]
        public void ShowItemCommand(Player player, string target, int index)
        {
            var targetCharacter = CharacterManager.GetCharacterNearby(player, target);

            if (targetCharacter != null) // Proceed only if exactly one character is found
            {
                var character = CharacterManager.PlayerCharacterPair[player];

                Item item = GetItemByIndex(character.Items, index);

                player.SendChatMessage($"{Colors.INFO}I-ai aratat {Colors.DARK_BLUE}{item.Name}{Colors.INFO} lui {targetCharacter.Name}.");
                targetCharacter.SendChatMessage($"{Colors.INFO}{character.Name} ti-a aratat {Colors.DARK_BLUE}{item.Name}{Colors.INFO}.");
            }
            else
            {
                // If no valid character is found, we already printed the characters in GetCharacterNearby
                player.SendChatMessage($"{Colors.ERROR}Nu s-a gasit un caracter valid sau este prea multe persoane.");
            }
        }

        [Command("use", Alias = "equip")]
        private void UseCommand(Player player, int index)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Item item = GetItemByIndex(character.Items, index);

            if (item == null)
            {
                player.SendChatMessage($"{Colors.ERROR}Item invalid.");
                return;
            }

            UseItem(character, item);
        }
        
        [Command("drop", "USAGE: /drop 'all'/[index] [index]")]
        private void DropCommand(Player player, string itemType = "", int index = -1, int amount = 1)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            if (itemType.ToLower() == "all")
            {
                // Drop all items
                foreach (var item in character.Items)
                {
                    character.ItemIdManager.ReclaimId(item.LocalId);
                }
                foreach (var item in character.Items)
                {
                    if (item.Type == "phone")
                        PhoneManager.phoneList.Remove(PhoneManager.GetPhoneFromItem(item));
                }
                character.Items.Clear();

                player.SendChatMessage($"{Colors.CRIMSON}You have dropped all items.");
            }
            else if (index >= 0) // Drop a specific item by index
            {
                if (!UtilMethods.TryGetElement(character.Items, index, out Item element))
                {
                    player.SendChatMessage($"{Colors.ERROR}The index does not exist in your inventory!");
                    return;
                }

                var item = character.Items[index];
                if (item == null)
                {
                    player.SendChatMessage($"{Colors.ERROR}You do not have an item with that ID.");
                    return;
                }

                item.Amount -= amount;

                if (item.Amount <= 0)
                {
                    character.Items.Remove(item);
                    character.ItemIdManager.ReclaimId(item.LocalId);
                }

                player.SendChatMessage($"{Colors.CRIMSON}You have dropped {amount}x {item.Name}.");
            }
            else
            {
                player.SendChatMessage($"{Colors.ERROR}Invalid usage. Use /drop all or /drop [index] [amount].");
            }
        }
        private bool GetCountable(string Type)
        {
            if (Type == "weapon")
                return false;
            else
                return true;
        }
        public static Character GetCharacterById(int characterId)
        {
            // Search for character using only the characterId
            return CharacterManager.loggedCharacters.FirstOrDefault(c => c.Id == characterId);
        }

        private void UseItem(Character character, Item usedItem)
        {
            Player player = CharacterManager.CharacterAccountPair[character].player;


            switch (usedItem.Type.ToLower())
            {
                case "weapon":
                    WeaponManager.HandleWeaponEquip(character, usedItem);
                    break;
                case "drink":
                    player.SendChatMessage($"{Colors.INFO}Ai baut {usedItem.Name}.");
                    player.Health += 10;
                    break;
                case "food":
                    player.SendChatMessage($"{Colors.INFO}Ai mancat {usedItem.Name}.");
                    player.Health += 25;
                    break;
                case "drug":
                    player.SendChatMessage($"{Colors.INFO}Ai bagat {usedItem.Name}, efectele vor aparea in cateva minute.");
                    player.Health += 50;
                    break;
                case "phone":
                    player.SendChatMessage($"Se deschide telefonul {usedItem.Name}");
                    Phone phone = new Phone(character, usedItem.Name, usedItem.Type, usedItem.PhoneNumber);
                    PhoneManager.SeeMessages(character, phone);
                    break;
                default:
                    player.SendChatMessage($"{Colors.ITEMS}Used item {usedItem.Name}.");
                    break;
            }
            // Handle item usage and removal
            if (usedItem.isStackable && usedItem.Type != "weapon" && usedItem.Type != "bullet")
            {
                usedItem.Amount--;
                if (usedItem.Amount <= 0)
                {
                    character.Items.Remove(usedItem);
                    character.ItemIdManager.ReclaimId(usedItem.LocalId);
                }
            }

            ItemDbManager.UpdateCharacterItems(character);
        }
        


        [RemoteEvent("CLIENT:SERVER:UseItem")]
        private void OnUseItem(Player player, string itemId)
        {
            try
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                int id = Convert.ToInt32(itemId);
                Item item = GetItemByIndex(character.Items, id);
                if (item != null)
                {
                    UseItem(character, item);
                    ShowUpdatedInventory(character);
                }
                else
                    player.SendChatMessage($"{Colors.ERROR} ID al itemului incorect.");
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnUseItem ERROR: {e.Message}\n{e.StackTrace}");
            }
        }

        [RemoteEvent("CLIENT:SERVER:EquipItem")]
        private void OnEquipItem(Player player, string itemId)
        {
            try
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                int id = Convert.ToInt32(itemId);
                Item item = GetItemByIndex(character.Items, id);
                if (item != null && item.Type.ToLower() == "weapon")
                {
                    UseItem(character, item); // Reuse UseItem since it already handles weapons
                    ShowUpdatedInventory(character);
                }
                else
                    player.SendChatMessage($"{Colors.ERROR} Acest item nu poate fi echipat.");

            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnEquipItem ERROR: {e.Message}\n{e.StackTrace}");
            }
        }

        [RemoteEvent("CLIENT:SERVER:ShowItem")]
        private void OnShowItem(Player player, int playerId, int itemId)
        {
            try
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                int id = Convert.ToInt32(itemId);
                Item item = GetItemByIndex(character.Items, id);

                var targetCharacter = CharacterManager.loggedCharacters.FirstOrDefault(c => c.GameId == playerId);

                targetCharacter.SendChatMessage($"{Colors.LIGHT_BLUE}{character.Name} iti arata {Colors.WHITE}{item.Name}{Colors.LIGHT_BLUE}.");

                player.SendChatMessage($"{Colors.LIGHT_BLUE}Ai aratat {Colors.WHITE}{item.Name}{Colors.LIGHT_BLUE} jucatorilor din jur.");
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnShowItem ERROR: {e.Message}\n{e.StackTrace}");
            }
        }
        public class NearbyCharactersData
        {
            public string Name { get; set; }
            public int Id { get; set; }

            public NearbyCharactersData(string name, int id)
            {
                Name = name;
                Id = id;
            }
        }

        [RemoteEvent("CLIENT:SERVER:GetNearbyCharacters")]
        private void GetNearbyCharacters(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(2f, player);
            var nearbyCharacters = new List<NearbyCharactersData>();
            foreach(var nearbyPlayer in nearbyPlayers)
            {
                var playerCharacter = CharacterManager.GetCharacter(nearbyPlayer);
                nearbyCharacters.Add(new NearbyCharactersData(playerCharacter.Name, playerCharacter.GameId));

            }
            if(nearbyCharacters != null)
            {
                var json = NAPI.Util.ToJson(nearbyCharacters);
                player.TriggerEvent("SERVER:CLIENT:PopulateNearbyCharacters", json);
            }
            else
            {
                character.SendErrorMessage("Nu ai playeri langa tine!");
            }
        }
        [RemoteEvent("CLIENT:SERVER:GiveItem")]
        private void OnGiveItem(Player player, int playerId, int itemId, int ammount)
        {
            try
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                int id = Convert.ToInt32(itemId);
                Item item = GetItemByIndex(character.Items, id);

                var targetCharacter = CharacterManager.loggedCharacters.FirstOrDefault(c => c.GameId == playerId);

                // Give the item
                Item receivedItem = new Item(targetCharacter, item.Name, item.Type, 1, item.isStackable);
                targetCharacter.Items.Add(receivedItem);

                // Remove from original player
                if (item.Amount <= 1)
                {
                    character.Items.Remove(item);
                    character.ItemIdManager.ReclaimId(item.LocalId);
                }
                else
                {
                    item.Amount--;
                }

                player.SendChatMessage($"{Colors.LIGHT_BLUE}I-ai dat {Colors.WHITE}{item.Name} {Colors.LIGHT_BLUE}lui {targetCharacter.Name}.");
                targetCharacter.SendChatMessage($"{Colors.LIGHT_BLUE}Ai primit {Colors.WHITE}{item.Name}{Colors.LIGHT_BLUE} de la {character.Name}.");
                ShowUpdatedInventory(character);

                ItemDbManager.UpdateCharacterItems(character);
                ItemDbManager.UpdateCharacterItems(targetCharacter);
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnGiveItem ERROR: {e.Message}\n{e.StackTrace}");
            }
        }

        [RemoteEvent("CLIENT:SERVER:DropItem")]
        private void OnDropItem(Player player, string itemId)
        {
            try
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                int id = Convert.ToInt32(itemId);
                Item item = GetItemByIndex(character.Items, id);

                if (item.Amount <= 1)
                {
                    character.Items.Remove(item);
                    character.ItemIdManager.ReclaimId(item.LocalId);
                }
                else
                {
                    item.Amount--;
                }

                player.SendChatMessage($"{Colors.CRIMSON}Ai aruncat {Colors.WHITE}{item.Name}{Colors.CRIMSON}.");
                ShowUpdatedInventory(character);
                ItemDbManager.UpdateCharacterItems(character);
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnDropItem ERROR: {e.Message}\n{e.StackTrace}");
            }
        }
        private void ShowUpdatedInventory(Character character)
        {
            List<ItemData> itemDataList = character.Items
                .Select(item => new ItemData(item.LocalId, item.Name, item.Type, item.Amount))
                .ToList();
            string json = NAPI.Util.ToJson(itemDataList);
            character.player.TriggerEvent("SERVER:CLIENT:UpdateInventory", json);
        }
        [RemoteEvent("CLIENT:SERVER:UpdateInventoryOrder")]
        private void OnUpdateInventoryOrder(Player player, string orderJson)
        {
            try
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                List<string> newOrder = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(orderJson);

                // Create a new ordered list based on the received order
                var reorderedItems = new List<Item>();
                foreach (var idStr in newOrder)
                {
                    int id = Convert.ToInt32(idStr);
                    var item = GetItemByIndex(character.Items, id);
                    if (item != null)
                    {
                        reorderedItems.Add(item);
                    }
                }

                // Add any items that weren't in the order list at the end
                foreach (var item in character.Items)
                {
                    if (!reorderedItems.Contains(item))
                    {
                        reorderedItems.Add(item);
                    }
                }

                character.Items = reorderedItems;
                ItemDbManager.UpdateCharacterItems(character);
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnUpdateInventoryOrder ERROR: {e.Message}\n{e.StackTrace}");
            }
        }

        public static Item GetItemByIndex(List<Item> items, int index)
        {
            bool foundItem = false;
            foreach (var item in items)
            {
                if (item.LocalId == index)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
