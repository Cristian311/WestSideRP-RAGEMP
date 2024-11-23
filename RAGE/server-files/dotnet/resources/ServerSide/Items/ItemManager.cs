using GTANetworkAPI;
using GTANetworkMethods;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.DB.CharacterDB;
using ServerSide.Interfaces;
using ServerSide.Items.Util;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Items
{
    internal class ItemManager : Script
    {
        private ItemManager()
        {
            WeaponHashMapper.LoadWeapons("json\\weapon\\weapon_names.json");
        }
        [ServerEvent(Event.PlayerDisconnected)]
        private void SaveCharItems(Player player, DisconnectionType type, string reason)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            ItemDbManager.UpdateCharacterItems(character);
        }
        [Command("inv")]
        private void cmd_inv(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            
            if(character.Items == null)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}You don't have any item.");
                return;
            }
            player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}Current items:");
            character.Items = FixItemsOrder(character.Items);
            foreach (var item in character.Items)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}[{item.localID}] {item.Name} - {item.Amount} units.");
            }
        }

        [Command("createitem")]
        private void cmd_createItem(Player player, string Name, string Type, int Ammount, string UsedFor = "")
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            var character = CharacterManager.PlayerCharacterPair[player];

            Name = StringFormatter.CapitalizeFirstLetter(Name); 
            UsedFor = StringFormatter.CapitalizeFirstLetter(UsedFor);
            player.SendChatMessage($"{Colors.COLOR_CHAT_ADMIN}Created item {Name} Type {Type} {Ammount} units.");
            bool Countable = getCountable(Type);

            if (character.Items == null)
            {
                character.Items = new List<Item>();
            }


            if (UsedFor != string.Empty && Type.ToLower() == "bullet")
            {
                character.Items.Add(new Item(character, Name, Type, Ammount, Countable, UsedFor));
            }
            else
            {
                if (Countable == false)
                {

                    for (int i = 1; i <= Ammount; i++)
                    {
                        character.Items.Add(new Item(character, Name, Type, 1, Countable));
                    }
                }
                else
                    character.Items.Add(new Item(character, Name, Type, Ammount, Countable));
            }
            ItemDbManager.UpdateCharacterItems(character);
        }
        [Command("createweapon")]
        private void cmd_createweapon(Player player, string Name, int Ammount = 1)
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
            }
            ItemDbManager.UpdateCharacterItems(character);
        }
        [Command("createdrug")]
        private void cmd_createdrug(Player player, string Name, int Ammount)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Name = StringFormatter.CapitalizeFirstLetter(Name);
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;

            player.SendChatMessage($"{Colors.COLOR_CHAT_LIME_GREEN}Created drug {Name} {Ammount} units.");
            character.Items.Add(new Item(character, Name, "drug", Ammount, true));
            ItemDbManager.UpdateCharacterItems(character);

        }
        [Command("giveitem")]
        public void cmd_giveitem(Player player, string target, int index, int ammount = 1)
        {
            var chars = CharacterManager.getCharByName(target);
            if (chars.Count == 1)
            {
                var targetCharacter = chars[0];
                var character = CharacterManager.PlayerCharacterPair[player];

                Item item = getItemByIndex(character.Items, index);
                if(item.Amount < ammount)
                {
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}N-ai {ammount} {item.Name}");
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
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name);
                }
            }
        }
        [Command("showitem")]
        public void cmd_showitem(Player player, string target, int index)
        {
            var chars = CharacterManager.getCharByNameAroundPlayer(20, player, target);
            if (chars.Count == 1)
            {
                var targetCharacter = chars[0];
                var character = CharacterManager.PlayerCharacterPair[player];

                Item item = getItemByIndex(character.Items, index);
                
                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}I-ai aratat {Colors.COLOR_CHAT_DARK_BLUE}{item.Name}{Colors.COLOR_CHAT_INFO} lui {targetCharacter.Name}.");
                targetCharacter.SendChatMessage($"{Colors.COLOR_CHAT_INFO}{character.Name}ti-a aratat {Colors.COLOR_CHAT_DARK_BLUE}{item.Name}{Colors.COLOR_CHAT_INFO}.");
            }
            else
            {
                bool found = false;
                foreach (var character in chars)
                {
                    found = true;
                    player.SendChatMessage(character.Name);
                }
                if (found == false)
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Caracterul nu este langa tine.");
            }
        }
        [Command("use", Alias = "equip", Description = "Usage: /use [itemID]")]
        private void cmd_use(Player player, int index)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Item item = getItemByIndex(character.Items, index);
            if (item != null)
            {
                //player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}You've used {item.Name}");
                UseItem(character, item);
            }
            else
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} ID al itemului incorect.");
        }

        [Command("unequip")]
        private void cmd_unequip(Player player)
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
        private void cmd_drop(Player player, string all = "")
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if(all == "all")
            {
                foreach(var item  in character.Items)
                {
                    character.ItemIdManager.ReclaimId(item.localID);
                }
                character.Items.Clear();
                
                player.SendChatMessage($"{Colors.COLOR_CHAT_CRIMSON}Ai aruncat toate itemele.");
            }
        }
        
        [Command("dropitem")]
        private void cmd_drop(Player player, int index, int ammount = 1)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
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
        private bool getCountable(string Type)
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
            
            switch(usedItem.Type.ToLower())
            {
                case "weapon":
                    var whiteWeapons = WeaponData.ExtractWeaponNames(WeaponData.WhiteWeaponData);
                    if(whiteWeapons.Contains("weapon_"+usedItem.Name.ToLower()))
                    {
                        WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(usedItem.Name);
                        player.GiveWeapon(weaponHash, 1);
                        character.Items.Remove(usedItem);
                        player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai echipat {weaponHash.ToString()}.");
                        return;
                    }
                    Item bullets = getBulletsOfWeapon(character, usedItem);
                    if (bullets != null)
                    {
                        if (bullets.Amount > 0)
                        {
                            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(usedItem.Name);
                            player.GiveWeapon(weaponHash, bullets.Amount);
                            character.currentWeapons.Add(weaponHash);
                            player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai echipat {weaponHash.ToString()}.");

                            character.Items.Remove(bullets);
                            character.Items.Remove(usedItem);
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
                default:    
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}Used item {usedItem.Name}.");
                    break;
            }
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
            ItemDbManager.UpdateCharacterItems(character);
        }
        private List<Item> FixItemsOrder(List<Item> items)
        {
            if (items == null || items.Count == 0)
                return new List<Item>();

            var groupedItems = new Dictionary<string, Item>();
            var result = new List<Item>();

            // Process items and group non-weapons by name and UsedFor
            foreach (var item in items)
            {
                if (item.Type != "weapon")
                {
                    string itemKey = item.Name?.Trim().ToLower();
                    string usedForKey = item.UsedFor?.Trim().ToLower();

                    if (string.IsNullOrEmpty(usedForKey))
                    {
                        // Handle case where UsedFor is not specified (no merging)
                        if (groupedItems.ContainsKey(itemKey))
                        {
                            groupedItems[itemKey].Amount += item.Amount;
                        }
                        else
                        {
                            groupedItems[itemKey] = new Item(item.currentCharacter, item.Name, item.Type, item.Amount, item.Countable, item.UsedFor)
                            {
                                localID = item.localID,  // Keep the current localID for now
                                Description = item.Description
                            };
                        }
                    }
                    else
                    {
                        // Check if we already have an item that matches the "UsedFor" key
                        if (groupedItems.ContainsKey(usedForKey))
                        {
                            var existingItem = groupedItems[usedForKey];

                            // Merge items if they are the same "UsedFor" type
                            if (existingItem.Name.ToLower().Contains(usedForKey) || item.Name.ToLower().Contains(usedForKey))
                            {
                                existingItem.Amount += item.Amount;
                            }
                            else
                            {
                                // If no match in names, just add them as different items
                                groupedItems[usedForKey] = new Item(item.currentCharacter, item.Name, item.Type, item.Amount, item.Countable, item.UsedFor)
                                {
                                    localID = item.localID,
                                    Description = item.Description
                                };
                            }
                        }
                        else
                        {
                            // Add a new item for this "UsedFor" type
                            groupedItems[usedForKey] = new Item(item.currentCharacter, item.Name, item.Type, item.Amount, item.Countable, item.UsedFor)
                            {
                                localID = item.localID,
                                Description = item.Description
                            };
                        }
                    }
                }
                else
                {
                    // Add weapons as separate items
                    result.Add(new Item(item.currentCharacter, item.Name, item.Type, 1, item.Countable, item.UsedFor)
                    {
                        localID = item.localID,  // Keep the current localID for now
                        Description = item.Description
                    });
                }
            }

            // Add grouped non-weapon items to the result list
            result.AddRange(groupedItems.Values);

            // Now, reassign localID to be sequential without gaps
            var sortedItems = result.OrderBy(item => item.localID).ToList();

            int newId = 0;
            foreach (var item in sortedItems)
            {
                item.localID = newId;
                newId++;
            }

            return sortedItems;
        }



        private Item getBulletsOfWeapon(Character character, Item weapon)
        {
            foreach (var item in character.Items)
            {
                if (item.Type == "bullet")
                {
                    if (StringFormatter.CapitalizeFirstLetter(item.UsedFor) == weapon.Name)
                        return item;
                }
            }
            return null;
        }
        private Item getItemByIndex(List<Item> items, int index)
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
