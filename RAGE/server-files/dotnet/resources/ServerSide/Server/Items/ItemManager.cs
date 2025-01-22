
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

        private static Dictionary<WeaponHash, string> equippedWeapons = new Dictionary<WeaponHash, string>();

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
                else if (item.UsedFor != string.Empty)
                    player.SendChatMessage($"{Colors.ITEMS}[{item.LocalId}] ({item.Type}) - {item.Name} - {item.Amount} units - Used for: {item.UsedFor}");
                else
                    player.SendChatMessage($"{Colors.ITEMS}[{item.LocalId}] ({item.Type}) - {item.Name} - {item.Amount} units.");
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
                    newItem.LocalId = character.ItemIdManager.AssignNewId(newItem);
                    character.Items.Add(newItem);
                }
            }
            else
            {
                // For countable items, create a single item
                var newItem = new Item(character, Name, Type, Ammount, true, UsedFor);
                newItem.LocalId = character.ItemIdManager.AssignNewId(newItem);
                character.Items.Add(newItem);
            }
            player.SendChatMessage($"{Colors.ADMIN}Created item {Name} Type {Type} {Ammount} units.");
            ItemDbManager.UpdateCharacterItems(character);
        }

        [Command("createweapon")]
        private void CreateWeaponCommand(Player player, string Name, int Ammount = 1)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            var character = CharacterManager.PlayerCharacterPair[player];
            Name = StringFormatter.CapitalizeFirstLetter(Name);

            player.SendChatMessage($"{Colors.ADMIN}Created weapon {Name} {Ammount} units.");
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

            player.SendChatMessage($"{Colors.LIME_GREEN}Created drug {Name} {Ammount} units.");
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
                    player.SendChatMessage($"{Colors.ERROR}N-ai {ammount} {item.Name}.");
                    return;
                }

                item.Amount -= ammount;
                if (item.Amount <= 0)
                {
                    character.Items.Remove(item);
                    character.ItemIdManager.ReclaimId(item.LocalId);
                }

                Item receivedItem = new Item(targetCharacter, item.Name, item.Type, ammount, item.isStackable, item.UsedFor);
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
        [Command("unequip")]
        private void UnequipCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.currentWeapons.Count == 0)
            {
                player.SendChatMessage($"{Colors.ERROR}No weapons equipped.");
                return;
            }

            foreach (var weaponHash in character.currentWeapons.ToList())
            {
                UnequipWeapon(player, character, weaponHash);
            }

            character.currentWeapons.Clear();
            ItemDbManager.UpdateCharacterItems(character);
        }

        private void UnequipWeapon(Player player, Character character, WeaponHash weaponHash)
        {
            if (!equippedWeapons.TryGetValue(weaponHash, out string weaponName))
            {
                NAPI.Util.ConsoleOutput($"Unmapped weapon hash: {weaponHash}");
                return;
            }

            int ammo = player.GetWeaponAmmo(weaponHash);

            // Create weapon item
            Item weaponItem = new Item(character, weaponName, "weapon", 1, false);
            weaponItem.LocalId = character.ItemIdManager.AssignNewId(weaponItem);
            character.Items.Add(weaponItem);

            // Create ammo item if there's any ammunition left
            if (ammo > 0)
            {
                Item ammoItem = new Item(character, $"{weaponName} bullets", "bullet", ammo, true, weaponName);
                ammoItem.LocalId = character.ItemIdManager.AssignNewId(ammoItem);
                character.Items.Add(ammoItem);
            }

            player.RemoveWeapon(weaponHash);
            equippedWeapons.Remove(weaponHash);
            player.SendChatMessage($"{Colors.INFO}Unequipped {weaponName} with {ammo} rounds remaining.");
        }

        // Helper methods
        private Item GetBulletsOfWeapon(Character character, Item weapon)
        {
            return character.Items.FirstOrDefault(item =>
                item.Type == "bullet" &&
                StringFormatter.CapitalizeFirstLetter(item.UsedFor) == weapon.Name);
        }

        [Command("drop")]
        private void DropCommand(Player player, string all = "")
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (all == "all")
            {
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

                player.SendChatMessage($"{Colors.CRIMSON}Ai aruncat toate itemele.");
            }
        }

        [Command("dropitem")]
        private void DropItemCommand(Player player, int index, int ammount = 1)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            if (!UtilMethods.TryGetElement(character.Items, index, out Item element))
            {
                player.SendChatMessage($"{Colors.ERROR}Index-ul nu exista in inventarul tau!");
                return;
            }

            var item = character.Items[index];
            if (item == null)
            {
                player.SendChatMessage($"{Colors.ERROR}Nu ai un item cu acel id.");
            }
            item.Amount -= ammount;

            if (item.Amount <= 0)
            {
                character.Items.Remove(item);
                character.ItemIdManager.ReclaimId(item.LocalId);
            }

            player.SendChatMessage($"{Colors.CRIMSON}Ai aruncat {ammount}{item.Name}.");
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
                    HandleWeaponEquip(player, character, usedItem);
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
        private void HandleWeaponEquip(Player player, Character character, Item weaponItem)
        {
            // Check if weapon is already equipped
            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(weaponItem.Name);
            if (character.currentWeapons.Contains(weaponHash))
            {
                player.SendChatMessage($"{Colors.ERROR}Already have this weapon equipped.");
                return;
            }

            // Handle white weapons (melee weapons)
            var whiteWeapons = WeaponData.ExtractWeaponNames(WeaponData.WhiteWeaponData);
            if (whiteWeapons.Contains("weapon_" + weaponItem.Name.ToLower()))
            {
                EquipWhiteWeapon(player, character, weaponItem, weaponHash);
                return;
            }

            // Handle regular weapons
            Item bullets = GetBulletsOfWeapon(character, weaponItem);
            if (bullets == null || bullets.Amount <= 0)
            {
                player.SendChatMessage($"{Colors.ERROR}No ammunition for this weapon.");
                return;
            }

            EquipWeaponWithAmmo(player, character, weaponItem, bullets, weaponHash);
        }

        private void EquipWhiteWeapon(Player player, Character character, Item weaponItem, WeaponHash weaponHash)
        {
            player.GiveWeapon(weaponHash, 1);
            character.currentWeapons.Add(weaponHash);
            equippedWeapons[weaponHash] = weaponItem.Name;

            // Remove from inventory
            character.Items.Remove(weaponItem);
            character.ItemIdManager.ReclaimId(weaponItem.LocalId);

            player.SendChatMessage($"{Colors.INFO}Equipped {weaponHash}.");
            ChatMessage.cmd_ame(player, "reaches for a weapon.");
        }

        private void EquipWeaponWithAmmo(Player player, Character character, Item weaponItem, Item bullets, WeaponHash weaponHash)
        {
            player.GiveWeapon(weaponHash, bullets.Amount);
            character.currentWeapons.Add(weaponHash);
            equippedWeapons[weaponHash] = weaponItem.Name;

            // Remove weapon and bullets from inventory
            character.Items.Remove(weaponItem);
            character.ItemIdManager.ReclaimId(weaponItem.LocalId);
            character.Items.Remove(bullets);
            character.ItemIdManager.ReclaimId(bullets.LocalId);

            player.SendChatMessage($"{Colors.INFO}Equipped {weaponHash} with {bullets.Amount} rounds.");
            ChatMessage.cmd_ame(player, "reaches for a weapon.");
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
                        nearbyPlayer.SendChatMessage($"{Colors.INFO}{character.Name} iti arata {Colors.DARK_BLUE}{item.Name}{Colors.INFO}.");
                    }
                }
                player.SendChatMessage($"{Colors.INFO}Ai aratat {Colors.DARK_BLUE}{item.Name}{Colors.INFO} jucatorilor din jur.");
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
                    player.SendChatMessage($"{Colors.ERROR}Trebuie sa fie doar o persoana langa tine pentru a da un item.");
                    return;
                }

                var targetPlayer = nearbyPlayers.First(p => p != player);
                var targetCharacter = CharacterManager.PlayerCharacterPair[targetPlayer];

                // Give the item
                Item receivedItem = new Item(targetCharacter, item.Name, item.Type, 1, item.isStackable, item.UsedFor);
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

                player.SendChatMessage($"{Colors.INFO}I-ai dat {Colors.DARK_BLUE}{item.Name} {Colors.INFO}lui {targetCharacter.Name}.");
                targetPlayer.SendChatMessage($"{Colors.INFO}Ai primit {Colors.DARK_BLUE}{item.Name} de la {Colors.INFO}{character.Name}.");

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

                player.SendChatMessage($"{Colors.CRIMSON}Ai aruncat {item.Name}.");
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
                    result.Add(new Item(item.CurrentCharacter, item.Name, item.Type, item.Amount, item.isStackable, item.UsedFor)
                    {
                        LocalId = item.LocalId,
                        Description = item.Description
                    });
                }
                else if (item.Type == "phone")
                {
                    var phone = new Phone(item.CurrentCharacter, item.Name, item.Type, item.PhoneNumber)
                    {
                        LocalId = item.LocalId,
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
                        otherItems[key] = new Item(item.CurrentCharacter, item.Name, item.Type, item.Amount, item.isStackable, item.UsedFor)
                        {
                            LocalId = item.LocalId,
                            Description = item.Description
                        };
                    }
                }
            }

            // Sort phones by phone number to ensure consistent ordering
            var sortedPhones = phoneItems.OrderBy(p => p.PhoneNumber).ToList();
            for (int i = 0; i < sortedPhones.Count; i++)
            {
                sortedPhones[i].LocalId = i;
                result.Add(sortedPhones[i]);
            }

            // Add other items
            result.AddRange(otherItems.Values.OrderBy(item => item.Name));

            // Reassign localID for non-phone and non-weapon items
            var otherItemsList = result.Where(item => item.Type != "weapon" && item.Type != "phone").ToList();
            for (int i = sortedPhones.Count; i < result.Count; i++)
            {
                result[i].LocalId = i;
            }

            return result;
        }

        private static string CreateUniqueKey(Item item)
        {
            string name = item.Name?.Trim().ToLower() ?? "";
            string usedFor = item.UsedFor?.Trim().ToLower() ?? "";

            return !string.IsNullOrEmpty(usedFor) ? usedFor : name;
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
