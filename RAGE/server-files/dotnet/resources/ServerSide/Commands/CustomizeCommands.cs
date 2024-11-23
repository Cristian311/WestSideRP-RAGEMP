using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.DB.CharacterDB;
using ServerSide.Interfaces;
using System.Collections.Generic;


namespace ServerSide.Commands
{
    internal class CustomizeCommands : Script
    {
        [Command("hair")]
        private void hairCommand(Player player)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            var HairJSON = NAPI.Util.ToJson(character.Hair);
            player.TriggerEvent("SERVER:CLIENT:showHairScreen", HairJSON);
        }
        [Command("facefeatures")]
        private void facefeaturesCommand(Player player)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            var FaceJSON = NAPI.Util.ToJson(character.Face);
            player.TriggerEvent("SERVER:CLIENT:showCustomizeCharScreen", FaceJSON);
        }
        [Command("changechar")]
        private void changecharCommand(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            string username = account.Name;

            CharacterManager.GetNameMoneyJson(account, out string namesJSON, out string moneyJSON);

            //display character interface with new character data
            player.TriggerEvent("SERVER:CLIENT:showCharInterface");
            player.TriggerEvent("SERVER:CLIENT:displayAccountInfo", username, namesJSON, moneyJSON);
        }
        [Command("clothes")]
        private void ClothesCommand(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            var character = CharacterManager.PlayerCharacterPair[player];
            player.TriggerEvent("SERVER:CLIENT:openClothesScreen");
        }
        [Command("shirtoff")]
        private void ShirtCommand(Player player)
        {
            player.SetClothes(3, 15, 0);
            player.SetClothes(11, 15, 0);
            player.SetClothes(8, 15, 0);
        }
    }
}
