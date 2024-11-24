using GTANetworkAPI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace ServerSide.Clothes
{
    internal class ClothesTextureData : Script
    {
        public Dictionary<string, FileSummary> results;
        private string gender;

        // Parameterless constructor
        public ClothesTextureData()
        {
            results = new Dictionary<string, FileSummary>();
        }

        // Method to set the gender and reload the appropriate files
        public void SetGender(string gender)
        {
            if (string.IsNullOrEmpty(gender))
                throw new ArgumentException("Gender cannot be null or empty.");

            // Set the gender
            this.gender = gender.ToLower(); // Make sure the gender is lowercase

            Console.WriteLine($"Gender set to: {this.gender}");

            // Reload the files based on the new gender
            LoadFiles();
        }

        // Method to load the files based on the gender
        private void LoadFiles()
        {
            string folderPath = @"json\v-clothingnames-master"; // Replace with your JSON files folder path
            string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");

            results.Clear(); // Clear the previous results

            Console.WriteLine("\nLoading files...");

            foreach (var file in jsonFiles)
            {
                string fileName = Path.GetFileName(file);

                // Check if the file is gender-specific or should always be included
                if (IsGenderSpecificFile(fileName))
                {
                    Console.WriteLine($"Including file: {fileName}");
                    var summary = ProcessJsonFile(file);

                    // Replace "_female_" with "_male_" in filenames for the results
                    if (gender == "female" && fileName.Contains("_female_"))
                    {
                        fileName = fileName.Replace("_female_", "_male_");
                    }

                    results.Add(fileName, summary);
                }
                else if (IsAlwaysIncludedFile(fileName))
                {
                    Console.WriteLine($"Including always-included file: {fileName}");
                    var summary = ProcessJsonFile(file);
                    results.Add(fileName, summary);
                }
                else
                {
                    Console.WriteLine($"Skipping file (does not match gender or always-included list): {fileName}");
                }
            }

            // Print the results after loading the files
            PrintResults(results);
        }

        // Method to determine if the file is gender-specific
        private bool IsGenderSpecificFile(string fileName)
        {
            if (string.IsNullOrEmpty(gender)) return false;

            string lowerFileName = fileName.ToLower();

            // Match filenames containing "_male_" or "_female_"
            if (gender == "male" && lowerFileName.Contains("_male_"))
            {
                Console.WriteLine($"File {fileName} matches gender 'male'");
                return true;
            }
            else if (gender == "female" && lowerFileName.Contains("_female_"))
            {
                Console.WriteLine($"File {fileName} matches gender 'female'");
                return true;
            }

            return false;
        }

        // Helper method to determine if a file should always be included
        private bool IsAlwaysIncludedFile(string fileName)
        {
            // Files like "masks.json" should always be included, regardless of gender
            string[] alwaysInclude = { "masks.json" };

            return alwaysInclude.Contains(fileName, StringComparer.OrdinalIgnoreCase);
        }

        private FileSummary ProcessJsonFile(string filePath)
        {
            string jsonContent = File.ReadAllText(filePath);

            // Parse the JSON as a JObject to manipulate its structure
            JObject jsonObject = JObject.Parse(jsonContent);

            int numberOfDrawables = jsonObject.Count; // Top-level keys represent drawables
            var drawableSummaries = new Dictionary<string, int>();

            foreach (var drawable in jsonObject.Properties())
            {
                // Ensure drawable.Value is a JObject
                if (drawable.Value is JObject drawableObject)
                {
                    int numberOfTextures = drawableObject.Count; // Count second-level keys
                    drawableObject["NumberOfTextures"] = numberOfTextures;

                    // Save drawable texture count
                    drawableSummaries.Add(drawable.Name, numberOfTextures);
                }
                else
                {
                    Console.WriteLine($"Warning: Drawable {drawable.Name} is not a valid JObject and was skipped.");
                }
            }

            Console.WriteLine($"Processed file: {Path.GetFileName(filePath)}");
            Console.WriteLine($"Number of Drawables: {numberOfDrawables}");

            // Return a summary of this file
            return new FileSummary
            {
                NumberOfDrawables = numberOfDrawables,
                DrawableSummaries = drawableSummaries
            };
        }

        private void PrintResults(Dictionary<string, FileSummary> results)
        {
            Console.WriteLine("\n--- Summary of All Files ---");
            foreach (var result in results)
            {
                Console.WriteLine($"File: {result.Key}");
                Console.WriteLine($"  Total Drawables: {result.Value.NumberOfDrawables}");

                foreach (var drawable in result.Value.DrawableSummaries)
                {
                    Console.WriteLine($"    Drawable {drawable.Key}: {drawable.Value} Textures");
                }
            }
            Console.WriteLine(NAPI.Util.ToJson(results));
        }
    }
}

public class FileSummary
{
    [JsonIgnore]
    public int NumberOfDrawables { get; set; }

    public Dictionary<string, int> DrawableSummaries { get; set; }
}
