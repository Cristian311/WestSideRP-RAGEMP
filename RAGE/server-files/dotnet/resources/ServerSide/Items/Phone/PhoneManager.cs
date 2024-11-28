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
        public static void SeeMessages(Player player, Phone phone)
        {
            if (phone.messages == null)
                return;
            foreach(var message in phone.messages)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}{message.SenderNumber}: {message.Message}.");
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
            int TargetPhoneNumber = int.Parse(Target);
            var PlayerPhone = GetPlayerPhone(player);
            var Message = new PhoneMessage(TargetPhoneNumber, PlayerPhone.PhoneNumber, MessageString);
            foreach (var phone in phoneList)
            {
                if (phone.PhoneNumber == Message.ReceiverNumber)
                {
                    phone.currentCharacter.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}Message from {Message.SenderNumber}: {Message.Message}");
                    phone.messages.Add(Message);
                    player.SendChatMessage($"{Colors.COLOR_CHAT_DARK_YELLOW}Message to {Message.ReceiverNumber}: {Message.Message}");
                    PhoneMessage phoneMessage = new PhoneMessage(TargetPhoneNumber, PlayerPhone.PhoneNumber, Message.Message);
                    phoneMessages.Add(phoneMessage);
                    break;
                }
            }
        }
        [Command("messages")]
        private void MessagesCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            foreach(var item in character.Items)
                if (item.Type == "phone")
                {
                    SeeMessages(player, GetPhoneFromItem(character, item));
                    break;
                }
        }
        [Command("createphone")]
        private void CreatePhoneCommand(Player player, string Name)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            var character = CharacterManager.PlayerCharacterPair[player];
            Name = StringFormatter.CapitalizeFirstLetter(Name);

            player.SendChatMessage($"{Colors.COLOR_CHAT_ADMIN}Created Phone {Name} {1} units.");
            bool Countable = false;
            Random random = new Random();
            int PhoneNumber = random.Next(10000, 99999);
            Phone phone = new Phone(character, Name, "phone", PhoneNumber);
            character.Items.Add(phone);
            phoneList.Add(phone);
            ItemDbManager.UpdateCharacterItems(character);
        }
    }
}
