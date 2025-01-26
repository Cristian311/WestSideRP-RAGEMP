using GTANetworkAPI;
using ServerSide.Server.DB.CharacterDB;
using System.Collections.Generic;
using System.IO;

namespace ServerSide.Characters.Customization.Tattoos
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

            //// Debugging output for all combined data
            //NAPI.Util.ConsoleOutput($"All Tattoo Data: {NAPI.Util.ToJson(allTattooData)}");

            //// Example: Output consolidated tattoo names
            //foreach (var name in tattooNames)
            //{
            //    NAPI.Util.ConsoleOutput($"Tattoo Name: {name}");
            //}
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
            if (character.Tattoos == null)
                character.Tattoos = new List<Decoration> { tattooDecoration };
            else if (character.Tattoos.Contains(tattooDecoration))
                return;
            else
                character.Tattoos.Add(tattooDecoration);
            if (character.TattooNames == null)
                character.TattooNames = new List<string>();
            character.TattooNames.Add(tattoo);

            player.SetDecoration(tattooDecoration);
        }
        public static void ApplyTattoos(Player player, Decoration[] tattoos)
        {
            player.SetDecoration(tattoos);
        }
        [RemoteEvent("CLIENT:SERVER:RemoveTattoo")]
        private void RemoveTattoo(Player player, string tattoo)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.Tattoos == null)
                return;
            string tattooHash = GetTattooHash(player, tattoo);
            string tattooCollection = GetTattooCollection(tattoo);

            Decoration tattooDecoration = new Decoration();
            tattooDecoration.Collection = NAPI.Util.GetHashKey(tattooCollection);
            tattooDecoration.Overlay = NAPI.Util.GetHashKey(tattooHash);
            if (character.Tattoos.Contains(tattooDecoration))
                character.Tattoos.Remove(tattooDecoration);
            character.TattooNames.Remove(tattoo);
            player.RemoveDecoration(tattooDecoration);
        }
        [RemoteEvent("CLIENT:SERVER:UpdateTattoo")]
        private void UpdateTattoo(Player player, string highlightJson)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var highlights = NAPI.Util.FromJson<List<string>>(highlightJson);
            character.TattooNames = highlights;
            UpdateCharDataDBManager.UpdateCharacterTattoos(character);
            NAPI.Util.ConsoleOutput($"Character {character.Name} updated tattoos: {highlightJson}");
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
            var character = CharacterManager.PlayerCharacterPair[player];

            var tattooZoneGrouped = new Dictionary<string, List<string>>();
            var tattooCollection = new Dictionary<string, string>();
            var tattooHash = new Dictionary<string, string>();

            // Group the tattoos by their zones
            foreach (var data in allTattooData)
            {
                if (!tattooZoneGrouped.ContainsKey(data.Zone))
                {
                    tattooZoneGrouped[data.Zone] = new List<string>();
                }
                tattooZoneGrouped[data.Zone].Add(data.LocalizedName);
                var hash = character.Gender ? data.HashNameMale : data.HashNameFemale;
                tattooHash[data.LocalizedName] = hash;
                tattooCollection[data.LocalizedName] = data.FileName;
            }

            // Convert the dictionary to a JSON string
            string tattooZoneGroupedJson = NAPI.Util.ToJson(tattooZoneGrouped);
            string CurrentTattoosJson = NAPI.Util.ToJson(character.TattooNames);
            string TattooCollectionJson = NAPI.Util.ToJson(tattooCollection);
            string TattooHashJson = NAPI.Util.ToJson(tattooHash);
            player.TriggerEvent("SERVER:CLIENT:ShowTattooScreen", tattooZoneGroupedJson, CurrentTattoosJson, TattooCollectionJson, TattooHashJson);

            //NAPI.Util.ConsoleOutput(tattooZoneGroupedJson);
        }
    }
}