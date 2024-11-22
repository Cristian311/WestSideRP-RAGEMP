using GTANetworkAPI;
using GTANetworkMethods;
using ServerSide.Characters;
using ServerSide.Interfaces;
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
            foreach (var item in character.Items)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}[{item.localID}] {item.Name} - {item.Amount} units");
            }
        }

        [Command("createitem")]
        private void cmd_createItem(Player player, string Name, string Type, int Ammount)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            player.SendChatMessage($"{Colors.COLOR_CHAT_ADMIN}Created item {Name} Type {Type} {Ammount} units");
            bool Countable;
            if (Type == "Weapon")
                Countable = false;
            else
                Countable = true;
            Item item = new Item(Name, Type, Ammount, Countable);
            character.Items.Add(item);
            
        }

        [Command("use")]
        private void cmd_use(Player player, int index)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var item = getItemByIndex(character.Items, index);
            if ( item != null)
            { 
                player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}You've used {item.Name}");
                UseItem(character, item);
            
            }

        }

        private void UseItem(Character character, Item usedItem)
        {
            var item = character.Items.FirstOrDefault(i => i == usedItem);
            var player = CharacterManager.CharacterAccountPair[character].player;
            
            switch(item.Type.ToLower())
            {
                case "weapon":
                    WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(item.Name);
                    player.GiveWeapon(weaponHash, 50);
                    break;
                default:
                    player.SendChatMessage($"{Colors.COLOR_CHAT_ITEMS}Used item {item.Name}");
                    break;

            }
            if (item != null)
            {
                if (!usedItem.Countable)
                    return;
                    // Decrease the item's amount
                    item.Amount--;

                // If the amount reaches zero, remove it from the list
                if (item.Amount <= 0)
                {
                    character.Items.Remove(item);
                }
            }
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
