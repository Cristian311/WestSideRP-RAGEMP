using GTANetworkAPI;
using GTANetworkInternals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerSide.Characters.Tattoos
{
    internal class TattooManager : Script
    {
        private List<TattooData> allTattooData = new List<TattooData>(); // Consolidated list
        private List<string> tattooNames = new List<string>(); // List of tattoo names
        private Dictionary<string, string> tattooNameToZone = new Dictionary<string, string>();

        public TattooManager()
        {
            string folderPath = "json\\v-tattoos-master"; // Folder containing JSON files
            string[] jsonFiles = Directory.GetFiles(folderPath, "*.json"); // Fetch all JSON files in the folder

            foreach (var path in jsonFiles)
            {
                string json = File.ReadAllText(path);
                List<TattooData> tattooData = NAPI.Util.FromJson<List<TattooData>>(json);

                string fileName = Path.GetFileNameWithoutExtension(path);

                foreach (var data in tattooData)
                {
                    data.FileName = fileName; // Assign the file name
                    allTattooData.Add(data); // Add to the consolidated list
                    tattooNames.Add(data.LocalizedName); // Add tattoo name to the list
                    tattooNameToZone[data.LocalizedName] = data.Zone; // Map tattoo name to zone

                }

                // Debugging output for each file
                NAPI.Util.ConsoleOutput($"Processed file: {fileName}, Total Tattoos: {tattooData.Count}");
            }

            // Debugging output for all combined data
            NAPI.Util.ConsoleOutput($"All Tattoo Data: {NAPI.Util.ToJson(allTattooData)}");

            // Example: Output consolidated tattoo names
            foreach (var name in tattooNames)
            {
                NAPI.Util.ConsoleOutput($"Tattoo Name: {name}");
            }
        }

        [RemoteEvent("CLIENT:SERVER:ApplyTattoo")]
        private void ApplyTattoo(Player player, string tattoo)
        {
            player.SendChatMessage(tattoo);
            string tattooHash = getTattooHash(player,tattoo);
            string tattooCollection = getTattooCollection(tattoo);
            Decoration tattooDecoration = new Decoration();
            tattooDecoration.Collection = NAPI.Util.GetHashKey(tattooCollection);
            tattooDecoration.Overlay = NAPI.Util.GetHashKey(tattooHash);
            player.SendChatMessage($"HASH: {tattooHash} COLLECTION: {tattooCollection}");
            player.SetDecoration(tattooDecoration);
        }

        private string getTattooCollection(string tattooName)
        {
            foreach (var data in allTattooData)
                if (data.LocalizedName == tattooName)
                    return data.FileName;
            return string.Empty;
        }
        private string getTattooHash(Player player, string tattooName)
        {
            var PlayerCharacter = CharacterManager.PlayerCharacterPair[player];
            foreach (var data in allTattooData)
            {
                if (data.LocalizedName == tattooName)
                {
                    if (PlayerCharacter.gender == true)
                        return data.HashNameMale;
                    else
                        return data.HashNameFemale;
                }
            }
            return string.Empty;
        }
        [Command("tattoo")]
        private void tattooCommand(Player player)
        {
            var tattooZoneGrouped = new Dictionary<string, List<string>>();

            // Group the tattoos by their zones
            foreach (var data in allTattooData)
            {
                if (!tattooZoneGrouped.ContainsKey(data.Zone))
                {
                    tattooZoneGrouped[data.Zone] = new List<string>();
                }
                tattooZoneGrouped[data.Zone].Add(data.LocalizedName);
            }

            // Convert the dictionary to a JSON string
            string json = NAPI.Util.ToJson(tattooZoneGrouped);

            // Send the JSON to the client-side event
            player.TriggerEvent("SERVER:CLIENT:showTattooScreen", json);

            // Log the JSON for debugging
            NAPI.Util.ConsoleOutput(json);
        }
    }
}
