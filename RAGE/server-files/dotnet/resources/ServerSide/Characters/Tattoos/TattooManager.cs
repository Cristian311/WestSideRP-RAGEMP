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
            var character = CharacterManager.PlayerCharacterPair[player];
            string tattooHash = GetTattooHash(player, tattoo);
            string tattooCollection = GetTattooCollection(tattoo);

            Decoration tattooDecoration = new Decoration();
            tattooDecoration.Collection = NAPI.Util.GetHashKey(tattooCollection);
            tattooDecoration.Overlay = NAPI.Util.GetHashKey(tattooHash);
            if(character.tattoos != null)
                character.tattoos.Add(tattooDecoration);
            player.SetDecoration(tattooDecoration);
        }
        public static void ApplyTattoos(Player player, List<Decoration> tattoosList)
        {
            if (tattoosList == null)
                return;
            Decoration[] tattoos = new Decoration[tattoosList.Count];
            for(int i = 0; i <= tattoosList.Count; i++)
                tattoos[i] = tattoosList[i];
            player.SetDecoration(tattoos);
        }
        [RemoteEvent("CLIENT:SERVER:RemoveTattoo")]
        private void RemoveTattoo(Player player, string tattoo)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.tattoos == null)
                return;
            string tattooHash = GetTattooHash(player, tattoo);
            string tattooCollection = GetTattooCollection(tattoo);

            Decoration tattooDecoration = new Decoration();
            tattooDecoration.Collection = NAPI.Util.GetHashKey(tattooCollection);
            tattooDecoration.Overlay = NAPI.Util.GetHashKey(tattooHash);
            if (character.tattoos.Contains(tattooDecoration))
                character.tattoos.Remove(tattooDecoration);
            player.RemoveDecoration(tattooDecoration);
        }

        private string GetTattooCollection(string tattooName)
        {
            foreach (var data in allTattooData)
                if (data.LocalizedName == tattooName)
                    return data.FileName;
            return string.Empty;
        }
        private string GetTattooHash(Player player, string tattooName)
        {
            var PlayerCharacter = CharacterManager.PlayerCharacterPair[player];
            foreach (var data in allTattooData)
            {
                if (data.LocalizedName == tattooName)
                {
                    if (PlayerCharacter.Gender == true)
                        return data.HashNameMale;
                    else
                        return data.HashNameFemale;
                }
            }
            return string.Empty;
        }
        [Command("tattoo")]
        private void TattooCommand(Player player)
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

            player.TriggerEvent("SERVER:CLIENT:showTattooScreen", json);

            NAPI.Util.ConsoleOutput(json);
        }
    }
}
