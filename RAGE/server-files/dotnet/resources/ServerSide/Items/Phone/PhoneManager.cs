using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Characters;
using ServerSide.DB.CharacterDB;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ServerSide.Items
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
            foreach(var message in phone.messages)
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}{message.SenderNumber}: {message.Message}.");
            }
        }
        private Phone GetPhoneFromItem(Character character, Item item)
        {
            if (item.Type != "phone")
                return null;
            Phone phone = new Phone(character, item.Name, "phone", item.PhoneNumber);
            return phone;
        }
        private Phone GetPlayerPhone(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            foreach(var item in character.Items)
            {
                if (item.Type == "phone")
                    return GetPhoneFromItem(character, item);
            }
            return null;
        }
        [Command("sms")]
        private void SmsCommand(Player player, string Target, string MessageString)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if(character.MainPhone == null)
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_ERROR} Nu ti-ai setat un telefon principal, foloseste comanda /setmainphone");
                return;
            }
            int TargetPhoneNumber = int.Parse(Target);
            var PlayerPhone = character.MainPhone; //GetPlayerPhone(player);
            var Message = new PhoneMessage(TargetPhoneNumber, PlayerPhone.PhoneNumber, MessageString);
            foreach (var phone in phoneList)
            {
                if (phone.PhoneNumber == Message.ReceiverNumber)
                {
                    phone.currentCharacter.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}Mesaj de la {Message.SenderNumber}: {Message.Message}");
                    phone.messages.Add(Message);
                    player.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}Mesaj catre {Message.ReceiverNumber}: {Message.Message}");
                    PhoneMessage phoneMessage = new PhoneMessage(TargetPhoneNumber, PlayerPhone.PhoneNumber, Message.Message);
                    phoneMessages.Add(phoneMessage);
                    break;
                }
            }
        }
        [Command("setmainphone")]
        private void SetMainPhoneCommand(Player player, int index)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.Items[index].Type != "phone")
            {
                character.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Acest item nu este un telefon!");
                return;
            }
            else
            {
                Item phoneItem = character.Items[index];
                Phone phone = new Phone(character, phoneItem.Name, "phone", phoneItem.PhoneNumber);
                character.MainPhone = phone;
                character.SendChatMessage($"{Colors.COLOR_CHAT_GREEN}Ai selected {phone.Name} ca telefon principal!");
            }
        }
        [Command("phones")]
        private void PhonesCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            bool found = false;
            character.Items = ItemManager.FixItemsOrder(character.Items);
            foreach (var item in character.Items)
            {
                if (item.Type == "phone")
                {
                    if(item == character.MainPhone)
                        character.SendChatMessage($"{Colors.COLOR_CHAT_GREEN}{item.localID}. {item.Name} - {item.PhoneNumber} [Main Phone]");
                    else
                        character.SendChatMessage($"{Colors.COLOR_CHAT_GREEN}{item.localID}. {item.Name} - {item.PhoneNumber}");
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
            foreach(var item in character.Items)
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

            player.SendChatMessage($"{Colors.COLOR_CHAT_ADMIN}Created Phone {Name} {1} units.");
            bool Countable = false;

            int PhoneNumber = random.Next(100000, 999999);
            while(PhoneNumberAvailable(PhoneNumber))
            {
                PhoneNumber = random.Next(100000, 999999);
            }
            Phone phone = new Phone(character, Name, "phone", PhoneNumber);

            character.Items.Add(phone);
            phoneList.Add(phone);
            ItemDbManager.UpdateCharacterItems(character);
        }
        private bool PhoneNumberAvailable(int phoneNumber)
        {
            foreach(var phone in phoneList)
            {
                if (phone.PhoneNumber == phoneNumber)
                    return false;
            }
            return true;
        }
    }
}
