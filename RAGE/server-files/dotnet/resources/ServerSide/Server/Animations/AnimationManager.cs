using GTANetworkAPI;
using ServerSide.Characters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerSide.Server.Animations
{
    public class AnimationManager : Script
    {
        public List<Animation> Animations { get; private set; }
        private string filePath = "Saved_Animations.txt";
        // Constructor that loads animations from a file
        public AnimationManager()
        {
            Animations = LoadFromFile(filePath);
        }

        [Command("anim")]
        private void AnimCommand(Player player, string anim)
        {
            var animation = GetAnimByName(anim);
            var character = CharacterManager.GetCharacter(player);

            character.PlayAnimation(animation.AnimDict, animation.AnimName, animation.Flag);
        }

        [Command("sa")]
        private void SaCommand(Player player)
        {
            player.StopAnimation();
        }

        // Synchronous method to load animations
        private List<Animation> LoadFromFile(string filePath)
        {
            var animations = new List<Animation>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file {filePath} does not exist.");

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length < 3) continue;

                    animations.Add(new Animation(
                        name: parts[0].Trim(':'),
                        animDict: parts[1],
                        animName: parts[2],
                        flag: parts.Length > 3 && int.TryParse(parts[3], out var flagValue) ? flagValue : 0
                    ));
                }
            }

            return animations;
        }

        // Example helper method to find an animation by name
        public Animation GetAnimByName(string name) =>
            Animations.Find(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
