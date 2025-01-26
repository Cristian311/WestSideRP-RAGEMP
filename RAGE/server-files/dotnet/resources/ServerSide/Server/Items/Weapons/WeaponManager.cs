using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Server.Commands;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.DB.CharacterDB;
using ServerSide.Server.Logger;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerSide.Server.Items.Weapons
{
    internal class WeaponManager
    {

        private static readonly Dictionary<string, string> WeaponBulletTypes = new Dictionary<string, string>()
        {
            { "Advancedrifle", "5.56mm" },
            { "Appistol", "9mm" },
            { "Assaultrifle", "7.62mm" },
            { "Assaultrifle_mk2", "7.62mm" },
            { "Assaultshotgun", "12-gauge" },
            { "Assaultsmg", "5.56mm" },
            { "Autoshotgun", "12-gauge" },
            { "Bullpuprifle", "5.8mm" },
            { "Bullpuprifle_mk2", "5.8mm" },
            { "Bullpupshotgun", "12-gauge" },
            { "Carbinerifle", "5.56mm" },
            { "Carbinerifle_mk2", "5.56mm" },
            { "Combatmg", "7.62mm" },
            { "Combatmg_mk2", "7.62mm" },
            { "Combatpdw", "5.7×28mm" },
            { "Combatpistol", ".45 ACP" },
            { "Doubleaction", ".38 Special" },
            { "Heavyshotgun", "12-gauge" },
            { "Heavysniper", ".50 BMG" },
            { "Heavysniper_mk2", ".50 BMG" },
            { "Machinepistol", "9mm" },
            { "Marksmanrifle", "7.62mm" },
            { "Marksmanrifle_mk2", "7.62mm" },
            { "Microsmg", "9mm" },
            { "Minigun", "7.62mm" },
            { "Pistol", ".45 ACP" },
            { "Pistol_mk2", "9mm" },
            { "Pistol50", ".50 AE" },
            { "Pumpshotgun", "12-gauge" },
            { "Pumpshotgun_mk2", "12-gauge" },
            { "Revolver", ".44 Magnum" },
            { "Revolver_mk2", ".45 Colt" },
            { "Smg", "9mm" },
            { "Smg_mk2", "5.7×28mm" },
            { "Specialcarbine", "7.62mm" },
            { "Specialcarbine_mk2", "7.62mm" },
            { "Vintagepistol", "7.65mm" }
        };

        [RequiresAdmin]
        [Command("createbullet")]
        private void CreateBulletCommand(Player player, string bulletType, int amount = 1)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;

            var character = CharacterManager.GetCharacter(player);
            var accountName = AccountManager.GetAccount(player).Name;
            bulletType = StringFormatter.CapitalizeFirstLetter(bulletType);

            if (!WeaponBulletTypes.ContainsValue(bulletType))
            {
                character.SendErrorMessage($"Eroare: '{bulletType}' este invalid.");
                return;
            }


            character.Items.Add(new Item(character, bulletType, "bullet", amount, true));
            ItemDbManager.UpdateCharacterItems(character);

            string adminMessage = $"{Colors.ADMIN}{accountName} a creat gloante de tipul {bulletType} ({amount} units).";
            AdminManager.SendChatMessageToAdmins(adminMessage);
            ServerLogger.Log(adminMessage);
        }

        [Command("bulletlist")]
        private void BulletListCommand(Player player)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;

            player.SendChatMessage($"{Colors.LIME_GREEN}--- Bullet Types and Associated Guns ---");

            // Dictionary to group weapons by bullet type
            var groupedByBulletType = new Dictionary<string, List<string>>();

            // Group weapons by bullet type
            foreach (var entry in WeaponBulletTypes)
            {
                var weaponName = entry.Key;
                var bulletType = entry.Value;

                if (!groupedByBulletType.ContainsKey(bulletType))
                    groupedByBulletType[bulletType] = new List<string>();

                groupedByBulletType[bulletType].Add(weaponName);
            }

            // Send grouped data to the player
            foreach (var bulletType in groupedByBulletType)
            {
                var weaponsList = string.Join(", ", bulletType.Value);
                player.SendChatMessage($"{Colors.LIGHT_BLUE}{bulletType.Key}: {Colors.YELLOW}{weaponsList}");
            }
        }
        [RequiresAdmin]
        [Command("createweapon")]
        private void CreateWeaponCommand(Player player, string Name, int Ammount = 1)
        {
            var character = CharacterManager.GetCharacter(player);
            var accountName = AccountManager.GetAccount(player).Name;

            Name = StringFormatter.CapitalizeFirstLetter(Name);

            AdminManager.SendChatMessageToAdmins($"{Colors.ADMIN}{accountName} a creat arma {Name} ({Ammount} units).");
            bool Countable = false;

            for (int i = 1; i <= Ammount; i++)
            {
                character.Items.Add(new Item(character, Name, "weapon", 1, Countable));
            }//update DB
            ItemDbManager.UpdateCharacterItems(character);
        }
        [Command("unequip")]
        public static void UnequipCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.currentWeapons.Count == 0)
            {
                player.SendChatMessage($"{Colors.ERROR}No weapons equipped.");
                return;
            }

            foreach (var weaponHash in character.currentWeapons.ToList())
            {
                UnequipWeapon(character, weaponHash);
            }

            character.currentWeapons.Clear();
            ItemDbManager.UpdateCharacterItems(character);
        }

        private static void UnequipWeapon(Character character, WeaponHash weaponHash)
        {
            var player = character.player;

            if (!character.equippedWeapons.TryGetValue(weaponHash, out string weaponName))
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
                if (WeaponBulletTypes.TryGetValue(weaponHash.ToString(), out string bulletType))
                {
                    Item ammoItem = new Item(character, bulletType, "bullet", ammo, true);
                    ammoItem.LocalId = character.ItemIdManager.AssignNewId(ammoItem);
                    character.Items.Add(ammoItem);
                }
                else
                {
                    NAPI.Util.ConsoleOutput($"Unmapped weapon hash for bullet type: {weaponHash}");
                }
            }

            // Remove the weapon from the player
            player.RemoveWeapon(weaponHash);
            character.equippedWeapons.Remove(weaponHash);

            player.SendChatMessage($"{Colors.INFO}Ai dezachipat {weaponName} cu {ammo} gloante ramase.");
        }


        // Helper methods
        private static Item GetBulletsOfWeapon(Character character, Item weapon)
        {
            // Get the bullet type based on the weapon's name
            if (WeaponBulletTypes.TryGetValue(weapon.Name, out string bulletType))
            {
                // Find the bullet item by its name (which matches the bullet type)
                return character.Items.FirstOrDefault(item =>
                    item.Type == "bullet" &&
                    string.Equals(item.Name, bulletType, StringComparison.OrdinalIgnoreCase));
            }

            return null; // No matching bullet type or bullet item found
        }

        public static void HandleWeaponEquip(Character character, Item weaponItem)
        {
            var player = character.player;
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
                EquipWhiteWeapon(character, weaponItem, weaponHash);
                return;
            }

            // Handle regular weapons
            var bullets = GetBulletsOfWeapon(character, weaponItem);
            if (bullets == null || bullets.Amount <= 0)
            {
                player.SendChatMessage($"{Colors.ERROR}No ammunition for this weapon.");
                return;
            }

            EquipWeaponWithAmmo(character, weaponItem, bullets, weaponHash);
        }

        private static void EquipWhiteWeapon(Character character, Item weaponItem, WeaponHash weaponHash)
        {
            var player = character.player;
            player.GiveWeapon(weaponHash, 1);
            character.currentWeapons.Add(weaponHash);
            character.equippedWeapons[weaponHash] = weaponItem.Name;

            // Remove from inventory
            character.Items.Remove(weaponItem);
            character.ItemIdManager.ReclaimId(weaponItem.LocalId);

            player.SendChatMessage($"{Colors.INFO}Equipped {weaponHash}.");
            ChatMessage.cmd_ame(player, "reaches for a weapon.");
        }

        private static void EquipWeaponWithAmmo(Character character, Item weaponItem, Item bullets, WeaponHash weaponHash)
        {
            var player = character.player;

            player.GiveWeapon(weaponHash, bullets.Amount);
            character.currentWeapons.Add(weaponHash);
            character.equippedWeapons[weaponHash] = weaponItem.Name;

            // Remove weapon and bullets from inventory
            character.Items.Remove(weaponItem);
            character.ItemIdManager.ReclaimId(weaponItem.LocalId);
            character.Items.Remove(bullets);
            character.ItemIdManager.ReclaimId(bullets.LocalId);

            player.SendChatMessage($"{Colors.INFO}Equipped {weaponHash} with {bullets.Amount} rounds.");
            ChatMessage.cmd_ame(player, "reaches for a weapon.");
        }
    }
}
