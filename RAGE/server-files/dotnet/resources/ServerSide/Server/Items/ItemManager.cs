
using GTANetworkAPI;
using GTANetworkMethods;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Server.Commands;
using ServerSide.Server.DB.CharacterDB;
using ServerSide.Server.Items.Util;
using ServerSide.Util;
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
            ItemDbManager.UpdateCharacterItems(character);
        }
        [Command("inv")]
        private void InvCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.Items == null)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu ai nici un item.");
                return;
            }
            character.Items = FixItemsOrder(character.Items);

            List<ItemData> itemDataList = character.Items
                .Select(item => new ItemData(item.localID, item.Name, item.Type, item.Amount))
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
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu ai nici un item.");
                return;
            }
            //
            character.Items = FixItemsOrder(character.Items); // fix items indexes in inventory

            player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}Itemele detinute in prezent:");
            foreach (var item in character.Items) // print items
            {
                if (item.Type == "phone")
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}[{item.localID}] (Phone) - {item.Name} - {item.PhoneNumber}");
                else if (item.UsedFor != string.Empty)
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}[{item.localID}] ({item.Type}) - {item.Name} - {item.Amount} units - Used for: {item.UsedFor}");
                else
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}[{item.localID}] ({item.Type}) - {item.Name} - {item.Amount} units.");
            }
        }

        [Command("createitem")]
        private void CreateItemCommand(Player player, string Name, string Type, int Ammount, string UsedFor = "")
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
                    var newItem = new Item(character, Name, Type, 1, false, UsedFor);
                    newItem.localID = character.ItemIdManager.AssignNewId(newItem);
                    character.Items.Add(newItem);
                }
            }
            else
            {
                // For countable items, create a single item
                var newItem = new Item(character, Name, Type, Ammount, true, UsedFor);
                newItem.localID = character.ItemIdManager.AssignNewId(newItem);
                character.Items.Add(newItem);
            }
            player.SendChatMessage($"{Colors.COLOR_CHAT_ADMIN}Created item {Name} Type {Type} {Ammount} units.");
            ItemDbManager.UpdateCharacterItems(character);
        }

        [Command("createweapon")]
        private void CreateWeaponCommand(Player player, string Name, int Ammount = 1)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            var character = CharacterManager.PlayerCharacterPair[player];
            Name = StringFormatter.CapitalizeFirstLetter(Name);

            player.SendChatMessage($"{Colors.COLOR_CHAT_ADMIN}Created weapon {Name} {Ammount} units.");
            bool Countable = false;

            for (int i = 1; i <= Ammount; i++)
            {
                character.Items.Add(new Item(character, Name, "weapon", 1, Countable));
            }//update DB
            ItemDbManager.UpdateCharacterItems(character);
        }
        [Command("createdrug")]
        private void CreateDrugCommand(Player player, string Name, int Ammount)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Name = StringFormatter.CapitalizeFirstLetter(Name);
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;

            player.SendChatMessage($"{Colors.COLOR_CHAT_LIME_GREEN}Created drug {Name} {Ammount} units.");
            character.Items.Add(new Item(character, Name, "drug", Ammount, true));

            //updatedb
            ItemDbManager.UpdateCharacterItems(character);

        }
        [Command("giveitem")]
        public void GiveItemCommand(Player player, string target, int index, int ammount = 1)
        {
            var targetCharacter = CharacterManager.GetCharacterNearby(player, target);

            if (targetCharacter != null) // Proceed only if exactly one character is found
            {
                var character = CharacterManager.PlayerCharacterPair[player];

                Item item = GetItemByIndex(character.Items, index);
                if (item.Amount < ammount)
                {
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}N-ai {ammount} {item.Name}.");
                    return;
                }

                item.Amount -= ammount;
                if (item.Amount <= 0)
                {
                    character.Items.Remove(item);
                    character.ItemIdManager.ReclaimId(item.localID);
                }

                Item receivedItem = new Item(targetCharacter, item.Name, item.Type, ammount, item.Countable, item.UsedFor);
                targetCharacter.Items.Add(receivedItem);
                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}I-ai dat {Colors.COLOR_CHAT_DARK_BLUE}{ammount} {item.Name} {Colors.COLOR_CHAT_INFO}lui {targetCharacter.Name}.");
                targetCharacter.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai primit {Colors.COLOR_CHAT_DARK_BLUE}{ammount} {item.Name} de la {Colors.COLOR_CHAT_INFO}{character.Name}.");
            }
            else
            {
                // If no valid character is found, we already printed the characters in GetCharacterNearby
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu s-a gasit un caracter valid sau sunt prea multe persoane in jur cu acelasi nume.");
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

                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}I-ai aratat {Colors.COLOR_CHAT_DARK_BLUE}{item.Name}{Colors.COLOR_CHAT_INFO} lui {targetCharacter.Name}.");
                targetCharacter.SendChatMessage($"{Colors.COLOR_CHAT_INFO}{character.Name} ti-a aratat {Colors.COLOR_CHAT_DARK_BLUE}{item.Name}{Colors.COLOR_CHAT_INFO}.");
            }
            else
            {
                // If no valid character is found, we already printed the characters in GetCharacterNearby
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu s-a gasit un caracter valid sau este prea multe persoane.");
            }
        }

        [Command("use", Alias = "equip", Description = "Usage: /use [itemID]")]
        private void UseCommand(Player player, int index)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Item item = GetItemByIndex(character.Items, index);
            if (item != null)
            {
                UseItem(character, item);
            }
            else
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} ID al itemului incorect.");
        }

        [Command("unequip")]
        private void UnequipCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            // Validate the player has weapons to unequip
            if (character.currentWeapons.Count == 0)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}N-ai nici o arma echipata.");
                return;
            }

            foreach (var weapon in character.currentWeapons)
            {
                string weaponName = StringFormatter.CapitalizeFirstLetter(weapon.ToString());

                if (string.IsNullOrEmpty(weaponName))
                {
                    NAPI.Util.ConsoleOutput($"Unmapped weapon hash: {weapon}");
                    continue;
                }

                //Create weapon and ammo data and add them to char
                Item weaponData = new Item(character, weaponName, "weapon", 1, false);
                character.Items.Add(weaponData);

                var ammo = player.GetWeaponAmmo(weapon);
                if (ammo > 0)
                {
                    Item ammoData = new Item(character, $"{weaponName} bullets", "bullet", ammo, true, weaponName);
                    character.Items.Add(ammoData);
                }

                player.RemoveWeapon(weapon);
            }
        }
        [Command("drop")]
        private void DropCommand(Player player, string all = "")
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (all == "all")
            {
                foreach (var item in character.Items)
                {
                    character.ItemIdManager.ReclaimId(item.localID);
                }
                foreach (var item in character.Items)
                {
                    if (item.Type == "phone")
                        PhoneManager.phoneList.Remove(PhoneManager.GetPhoneFromItem(item));
                }
                character.Items.Clear();

                player.SendChatMessage($"{Colors.COLOR_CHAT_CRIMSON}Ai aruncat toate itemele.");
            }
        }

        [Command("dropitem")]
        private void DropItemCommand(Player player, int index, int ammount = 1)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            if (!UtilMethods.TryGetElement(character.Items, index, out Item element))
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Index-ul nu exista in inventarul tau!");
                return;
            }

            var item = character.Items[index];
            if (item == null)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu ai un item cu acel id.");
            }
            item.Amount -= ammount;

            if (item.Amount <= 0)
            {
                character.Items.Remove(item);
                character.ItemIdManager.ReclaimId(item.localID);
            }

            player.SendChatMessage($"{Colors.COLOR_CHAT_CRIMSON}Ai aruncat {ammount}{item.Name}.");
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
                    var whiteWeapons = WeaponData.ExtractWeaponNames(WeaponData.WhiteWeaponData);
                    if (whiteWeapons.Contains("weapon_" + usedItem.Name.ToLower()))
                    {
                        WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(usedItem.Name);
                        player.GiveWeapon(weaponHash, 1);
                        character.currentWeapons.Add(weaponHash);

                        character.Items.Remove(usedItem);
                        character.ItemIdManager.ReclaimId(usedItem.localID);

                        player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai echipat {weaponHash.ToString()}.");
                        ChatMessage.ameCommand(player, "intinde mana pentru o arma.");
                        return;
                    }
                    Item bullets = GetBulletsOfWeapon(character, usedItem);
                    if (bullets != null)
                    {
                        if (bullets.Amount > 0)
                        {
                            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(usedItem.Name);
                            player.GiveWeapon(weaponHash, bullets.Amount);
                            character.currentWeapons.Add(weaponHash);

                            character.Items.Remove(bullets);
                            character.ItemIdManager.ReclaimId(bullets.localID);
                            character.Items.Remove(usedItem);
                            character.ItemIdManager.ReclaimId(usedItem.localID);

                            player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai echipat {weaponHash.ToString()}.");
                            ChatMessage.ameCommand(player, "intinde mana pentru o arma.");
                        }
                        else
                        {
                            player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}Nu ai gloante pentru acest tip de arma.");
                        }
                    }
                    else
                    {
                        player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu ai gloante pentru acest tip de arma.");
                    }
                    break;
                case "drink":
                    player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai baut {usedItem.Name}.");
                    player.Health += 10;
                    break;
                case "food":
                    player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai mancat {usedItem.Name}.");
                    player.Health += 25;
                    break;
                case "drug":
                    player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai bagat {usedItem.Name}, efectele vor aparea in cateva minute.");
                    player.Health += 50;
                    break;
                case "phone":
                    player.SendChatMessage($"Se deschide telefonul {usedItem.Name}");
                    Phone phone = new Phone(character, usedItem.Name, usedItem.Type, usedItem.PhoneNumber);
                    PhoneManager.SeeMessages(character, phone);
                    break;
                default:
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}Used item {usedItem.Name}.");
                    break;
            }
            // Handle item usage and removal
            if (usedItem != null)
            {
                if (!usedItem.Countable)
                    return;
                // Decrease the item's amount
                usedItem.Amount--;

                // If the amount reaches zero, remove it from the list
                if (usedItem.Amount <= 0)
                {
                    character.Items.Remove(usedItem);
                    character.ItemIdManager.ReclaimId(usedItem.localID);
                }
            }

            // Reassign IDs to ensure they remain sequential
            //character.ItemIdManager.ReassignSequentialIds(character.Items);
            ItemDbManager.UpdateCharacterItems(character);
        }

        // Add these methods to the ItemManager class:

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
                }
                else
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} ID al itemului incorect.");
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
                }
                else
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} Acest item nu poate fi echipat.");
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnEquipItem ERROR: {e.Message}\n{e.StackTrace}");
            }
        }

        [RemoteEvent("CLIENT:SERVER:ShowItem")]
        private void OnShowItem(Player player, string itemId)
        {
            try
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                int id = Convert.ToInt32(itemId);
                Item item = GetItemByIndex(character.Items, id);

                var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(2f, player);
                foreach (var nearbyPlayer in nearbyPlayers)
                {
                    if (nearbyPlayer != player)
                    {
                        nearbyPlayer.SendChatMessage($"{Colors.COLOR_CHAT_INFO}{character.Name} iti arata {Colors.COLOR_CHAT_DARK_BLUE}{item.Name}{Colors.COLOR_CHAT_INFO}.");
                    }
                }
                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai aratat {Colors.COLOR_CHAT_DARK_BLUE}{item.Name}{Colors.COLOR_CHAT_INFO} jucatorilor din jur.");
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnShowItem ERROR: {e.Message}\n{e.StackTrace}");
            }
        }

        [RemoteEvent("CLIENT:SERVER:GiveItem")]
        private void OnGiveItem(Player player, string itemId)
        {
            try
            {
                var character = CharacterManager.PlayerCharacterPair[player];
                int id = Convert.ToInt32(itemId);
                Item item = GetItemByIndex(character.Items, id);

                var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(30f, player);
                if (nearbyPlayers.Count != 2) // player + 1 target
                {
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Trebuie sa fie doar o persoana langa tine pentru a da un item.");
                    return;
                }

                var targetPlayer = nearbyPlayers.First(p => p != player);
                var targetCharacter = CharacterManager.PlayerCharacterPair[targetPlayer];

                // Give the item
                Item receivedItem = new Item(targetCharacter, item.Name, item.Type, 1, item.Countable, item.UsedFor);
                targetCharacter.Items.Add(receivedItem);

                // Remove from original player
                if (item.Amount <= 1)
                {
                    character.Items.Remove(item);
                    character.ItemIdManager.ReclaimId(item.localID);
                }
                else
                {
                    item.Amount--;
                }

                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}I-ai dat {Colors.COLOR_CHAT_DARK_BLUE}{item.Name} {Colors.COLOR_CHAT_INFO}lui {targetCharacter.Name}.");
                targetPlayer.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai primit {Colors.COLOR_CHAT_DARK_BLUE}{item.Name} de la {Colors.COLOR_CHAT_INFO}{character.Name}.");

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
                    character.ItemIdManager.ReclaimId(item.localID);
                }
                else
                {
                    item.Amount--;
                }

                player.SendChatMessage($"{Colors.COLOR_CHAT_CRIMSON}Ai aruncat {item.Name}.");
                ItemDbManager.UpdateCharacterItems(character);
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"OnDropItem ERROR: {e.Message}\n{e.StackTrace}");
            }
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
        public static List<Item> FixItemsOrder(List<Item> items)
        {
            if (items == null || items.Count == 0)
                return new List<Item>();

            var result = new List<Item>();
            var phoneItems = new List<Phone>();
            var otherItems = new Dictionary<string, Item>();

            foreach (var item in items)
            {
                if (item.Type == "weapon")
                {
                    result.Add(new Item(item.currentCharacter, item.Name, item.Type, item.Amount, item.Countable, item.UsedFor)
                    {
                        localID = item.localID,
                        Description = item.Description
                    });
                }
                else if (item.Type == "phone")
                {
                    var phone = new Phone(item.currentCharacter, item.Name, item.Type, item.PhoneNumber)
                    {
                        localID = item.localID,
                        Description = item.Description
                    };
                    phoneItems.Add(phone);
                }
                else
                {
                    string key = CreateUniqueKey(item);

                    if (otherItems.TryGetValue(key, out var existingItem))
                    {
                        existingItem.Amount += item.Amount;
                    }
                    else
                    {
                        otherItems[key] = new Item(item.currentCharacter, item.Name, item.Type, item.Amount, item.Countable, item.UsedFor)
                        {
                            localID = item.localID,
                            Description = item.Description
                        };
                    }
                }
            }

            // Sort phones by phone number to ensure consistent ordering
            var sortedPhones = phoneItems.OrderBy(p => p.PhoneNumber).ToList();
            for (int i = 0; i < sortedPhones.Count; i++)
            {
                sortedPhones[i].localID = i;
                result.Add(sortedPhones[i]);
            }

            // Add other items
            result.AddRange(otherItems.Values.OrderBy(item => item.Name));

            // Reassign localID for non-phone and non-weapon items
            var otherItemsList = result.Where(item => item.Type != "weapon" && item.Type != "phone").ToList();
            for (int i = sortedPhones.Count; i < result.Count; i++)
            {
                result[i].localID = i;
            }

            return result;
        }

        private static string CreateUniqueKey(Item item)
        {
            string name = item.Name?.Trim().ToLower() ?? "";
            string usedFor = item.UsedFor?.Trim().ToLower() ?? "";

            return !string.IsNullOrEmpty(usedFor) ? usedFor : name;
        }
        private Item GetBulletsOfWeapon(Character character, Item weapon)
        {
            foreach (var item in character.Items)
            {
                if (item.Type == "bullet")
                {
                    if (StringFormatter.CapitalizeFirstLetter(item.UsedFor) == weapon.Name) //check if the bullet's used for param is the weapon
                        return item;
                }
            }
            return null;
        }
        public static Item GetItemByIndex(List<Item> items, int index)
        {
            bool foundItem = false;
            foreach (var item in items)
            {
                if (item.localID == index)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
