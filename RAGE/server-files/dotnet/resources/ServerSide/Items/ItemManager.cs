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
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}You don't have any item");
                return;
            }
            player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}Current items:");
            character.Items = FixItemsOrder(character.Items);
            foreach (var item in character.Items)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}[{item.localID}] {item.Name} - {item.Amount} units");
            }
        }

        [Command("createitem")]
        private void cmd_createItem(Player player, string Name, string Type, int Ammount, string UsedFor = "")
        {

            var character = CharacterManager.PlayerCharacterPair[player];

            Name = StringFormatter.CapitalizeFirstLetter(Name); 
            UsedFor = StringFormatter.CapitalizeFirstLetter(UsedFor);
            player.SendChatMessage($"{Colors.COLOR_CHAT_ADMIN}Created item {Name} Type {Type} {Ammount} units");
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
        private void cmd_createweapon(Player player, string Name, int Ammount)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Name = StringFormatter.CapitalizeFirstLetter(Name);
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            
            player.SendChatMessage($"{Colors.COLOR_CHAT_ADMIN}Created weapon {Name} {Ammount} units");
            bool Countable = false;

            for (int i = 1; i <= Ammount; i++)
            {
                character.Items.Add(new Item(character, Name, "weapon", Ammount, Countable));
            }
        }
        [Command("creatdrug")]
        private void cmd_createdrug(Player player, string Name, int Ammount)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Name = StringFormatter.CapitalizeFirstLetter(Name);
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;

            player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}Created drug {Name} {Ammount} units");
            bool Countable = false;

            for (int i = 1; i <= Ammount; i++)
            {
                character.Items.Add(new Item(character, Name, "drug", Ammount, Countable));
            }
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
                    player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}N-ai {ammount} {item.Name}");
                    return;
                }    
                item.Amount -= ammount;
                if (item.Amount <= 0)
                {
                    character.Items.Remove(item);
                    character.ItemIdManager.ReclaimId(item.Id);
                }
                Item receivedItem = new Item(targetCharacter, item.Name, item.Type, ammount, item.Countable, item.UsedFor);
                targetCharacter.Items.Add(receivedItem);
                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}I-ai dat {ammount} {item.Name} lui {targetCharacter.Name}");
                targetCharacter.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai primit {ammount} {item.Name} de la {character.Name}");
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
                
                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}I-ai aratat {Colors.COLOR_CHAT_DARK_BLUE}{item.Name}{Colors.COLOR_CHAT_INFO} lui {targetCharacter.Name}");
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
                    player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}Caracterul nu este langa tine.");
            }
        }
        [Command("use")]
        private void cmd_use(Player player, int index)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Item item = getItemByIndex(character.Items, index);
            if ( item != null)
            { 
                //player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}You've used {item.Name}");
                UseItem(character, item);
            }
        }

        [Command("unequip")]
        private void cmd_unequip(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            // Validate the player has weapons to unequip
            if (character.currentWeapons.Count == 0)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}N-ai nici o arma echipata.");
                return;
            }

            foreach (var weapon in character.currentWeapons)
            {
                string weaponName = StringFormatter.CapitalizeFirstLetter(weapon.ToString());

                if (string.IsNullOrEmpty(weaponName))
                {
                    Console.WriteLine($"Unmapped weapon hash: {weapon}");
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
        private void cmd_drop(Player player, string all = "", int index = -1)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if(all == "all")
            {
                character.Items.Clear();
                player.SendChatMessage($"{Colors.COLOR_CHAT_CRIMSON}Ai aruncat toate itemele");
            }
            else if(index < 0)
            {

                player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}Selecteaza ID-ul itemului.");
            }    
            else
            {
                var item = character.Items[index];
                character.Items.RemoveAt(index);
                character.ItemIdManager.ReclaimId(item.Id);
                player.SendChatMessage($"{Colors.COLOR_CHAT_CRIMSON}Ai aruncat {item.Name}");
            }
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
                    Item bullets = getBulletsOfWeapon(character, usedItem);
                    if (bullets != null)
                    {
                        if (bullets.Amount > 0)
                        {
                            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(usedItem.Name);
                            player.GiveWeapon(weaponHash, bullets.Amount);
                            character.currentWeapons.Add(weaponHash);
                            player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai echipat {weaponHash.ToString()}");

                            character.Items.Remove(bullets);
                            character.Items.Remove(usedItem);
                        }
                        else
                        {
                            player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}Nu ai gloante pentru acest tip de arma");
                        }
                    }
                    else
                    {
                        player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}Nu ai gloante pentru acest tip de arma");
                    }
                    break;
                case "drink":
                    player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai baut {usedItem.Name}");
                    player.Health += 10;
                    break;
                case "food":
                    player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai mancat {usedItem.Name}");
                    player.Health += 25;
                    break;
                case "drug":
                    player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}Ai bagat {usedItem.Name}, efectele vor aparea in cateva minute");
                    player.Health += 50;
                    break;
                default:    
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}Used item {usedItem.Name}");
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
                    character.ItemIdManager.ReclaimId(usedItem.Id);
                }
            }
            ItemDbManager.UpdateCharacterItems(character);
        }
        private List<Item> FixItemsOrder(List<Item> items)
        {
            if (items == null || items.Count == 0)
                return new List<Item>();

            var groupedItems = new Dictionary<string, Item>();
            foreach (var item in items)
            {
                string itemKey = item.Name.Trim().ToLower();

                if (groupedItems.ContainsKey(itemKey))
                {
                    groupedItems[itemKey].Amount += item.Amount;
                }
                else
                {
                    groupedItems[itemKey] = item;
                }
            }

            return new List<Item>(groupedItems.Values);
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
