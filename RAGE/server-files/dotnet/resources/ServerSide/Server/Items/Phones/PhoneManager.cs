using GTANetworkAPI;
using Org.BouncyCastle.Bcpg;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.Server.DB.CharacterDB;
using ServerSide.Server.Items;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ServerSide
{
    internal class PhoneManager : Script
    {
        public static List<Phone> phoneList = new List<Phone>();
        public static List<PhoneMessage> phoneMessages = new List<PhoneMessage>();
        public static void SeeMessages(Character character, Phone phone)
        {
            if (phone.messages == null)
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}Nu ai nici un mesaj.");
                return;
            }
            foreach (var message in phone.messages)
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}{message.SenderNumber}: {message.Message}.");
            }
        }
        public static Phone GetPhoneFromItem(Item item)
        {
            if (item.Type != "phone")
                throw new InvalidOperationException("Cannot convert non-phone item to Phone.");

            // Use the item's properties to create a new Phone object
            return new Phone(
                item.currentCharacter,
                item.Name,
                item.Type,
                item.PhoneNumber, // Use the PhoneNumber property from the item
                item.Amount,
                item.Countable,
                item.UsedFor
            );
        }

        private Phone GetPlayerPhone(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            foreach (var item in character.Items)
            {
                if (item.Type == "phone")
                    return GetPhoneFromItem(item);
            }
            return null;
        }
        [Command("sms")]
        private void SmsCommand(Player player, string Target, string MessageString)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.MainPhone == null)
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} Nu ti-ai setat un telefon principal, foloseste comanda /setmainphone");
                return;
            }

            if (!int.TryParse(Target, out int TargetPhoneNumber))
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} Numar de telefon invalid.");
                return;
            }

            var PlayerPhone = character.MainPhone; //GetPlayerPhone(player);
            var Message = new PhoneMessage(TargetPhoneNumber, PlayerPhone.PhoneNumber, MessageString);

            bool messageSent = false;
            foreach (var phone in phoneList)
            {
                Console.WriteLine($"Phone: {phone.PhoneNumber}, Character: {phone.currentCharacter?.Name}");

                if (phone.PhoneNumber == Message.ReceiverNumber)
                {
                    if (phone.currentCharacter == null)
                    {
                        player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} Telefonul tinta nu este activ.");
                        return;
                    }

                    phone.currentCharacter.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}Mesaj de la {Message.SenderNumber}: {Message.Message}");
                    phone.messages ??= new List<PhoneMessage>();
                    phone.messages.Add(Message);

                    player.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}Mesaj catre {Message.ReceiverNumber}: {Message.Message}");
                    phoneMessages.Add(Message);

                    messageSent = true;
                    break;
                }
            }

            if (!messageSent)
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} Numarul de telefon nu a fost gasit.");
            }
        }

        [Command("setmainphone")]
        public static void SetMainPhoneCommand(Player player, int index)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            Item phoneItem = ItemManager.GetItemByIndex(character.Items, index);
            if (phoneItem == null)
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Acest item nu este un telefon!");
                return;
            }
            if (phoneItem.Type != "phone")
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Acest item nu este un telefon!");
                return;
            }
            else
            {
                Phone phone = GetPhoneFromItem(phoneItem);
                character.MainPhone = phone;
                character.SendChatMessage($"{Colors.COLOR_CHAT_GREEN}Ai selectat {phone.Name} ca telefon principal!");

                UpdateCharDataDBManager.UpdateCharacterPhone(character, phoneItem.Id);
            }
        }
        [Command("phones")]
        private void PhonesCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            bool found = false;
            //character.Items = ItemManager.FixItemsOrder(character.Items);
            foreach (var item in character.Items)
            {
                if (item.Type == "phone")
                {
                    var phone = GetPhoneFromItem(item);
                    if (character.MainPhone != null && item.Name == character.MainPhone.Name) // !!!!! NEEDS FIX ASAP IT SHOULD CHECK BY ID NOT NAME, BUT ITS BUGGED
                        character.SendChatMessage($"{Colors.COLOR_CHAT_GREEN}{item.localID}. {phone.Name} - {phone.PhoneNumber} [Main Phone]");
                    else
                        character.SendChatMessage($"{Colors.COLOR_CHAT_GREEN}{item.localID}. {phone.Name} - {phone.PhoneNumber}");
                    found = true;
                }
            }
            if (found == false)
                character.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu ai nici un telefon!");
        }
        [Command("messages")]
        private void MessagesCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            foreach (var item in character.Items)
                if (item.Type == "phone")
                {
                    SeeMessages(character, character.MainPhone);
                    break;
                }
        }
        private static readonly Random random = new Random();

        [Command("createphone")]
        private void CreatePhoneCommand(Player player, string Name)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            var character = CharacterManager.PlayerCharacterPair[player];
            Name = StringFormatter.CapitalizeFirstLetter(Name);

            player.SendChatMessage($"{Colors.COLOR_CHAT_LIGHT_RED}Created phone {Name} {1} units.");

            int PhoneNumber = random.Next(100000, 999999);
            while (!PhoneNumberAvailable(PhoneNumber))
            {
                player.SendChatMessage(".");
                PhoneNumber = random.Next(100000, 999999);
            }

            // Create the phone with the specific phone number
            Phone phone = new Phone(character, Name, "phone", PhoneNumber);

            // Ensure the phone number is set in the item as well
            Item phoneItem = new Item(character, Name, "phone", 1, false)
            {
                PhoneNumber = PhoneNumber
            };

            character.SendChatMessage(PhoneNumber.ToString());
            character.Items.Add(phoneItem);
            phoneList.Add(phone);
            ItemDbManager.UpdateCharacterItems(character);
        }
        private bool PhoneNumberAvailable(int phoneNumber)
        {
            return !phoneList.Any(phone => phone.PhoneNumber == phoneNumber);
        }

    }
}
