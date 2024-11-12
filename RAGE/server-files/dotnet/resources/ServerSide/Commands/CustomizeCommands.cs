using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.DB;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Commands
{
    internal class CustomizeCommands : Script
    {
        [Command("hair")]
        private void hairCommand(Player player)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            player.TriggerEvent("SERVER:CLIENT:showHairScreen", character.HairJSON);
        }
        [Command("facefeatures")]
        private void facefeaturesCommand(Player player)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            player.TriggerEvent("SERVER:CLIENT:showCustomizeCharScreen", character.CharacterFaceDetailsJSON);
        }
        [Command("changechar")]
        private void changecharCommand(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            string username = account.Name;

            CharacterDbManager characterDb = new CharacterDbManager();
            List<ICharacter> characters = characterDb.getCharacters(account);

            CharacterManager cm = new CharacterManager();
            cm.getNamesAndMoneyForCEF(characters, cm.getRequiredCount(characters), out string[] names, out int[] money);

            string namesJSON = NAPI.Util.ToJson(names);
            string moneyJSON = NAPI.Util.ToJson(money);

            //display character interface with new character data
            player.TriggerEvent("SERVER:CLIENT:showCharInterface");
            player.TriggerEvent("SERVER:CLIENT:displayAccountInfo", username, namesJSON, moneyJSON);
        }
        [Command("clothes")]
        private void clothesCommand(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            var character = CharacterManager.PlayerCharacterPair[player];
            player.TriggerEvent("SERVER:CLIENT:openClothesScreen");
        }
    }
}
