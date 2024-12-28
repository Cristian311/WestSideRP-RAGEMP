using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;
namespace ServerSide.Server.Items.Util
{
    public class WeaponHashMapper
    {
        private static Dictionary<string, string> WeaponHashToName;

        public static void LoadWeapons(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                WeaponHashToName = NAPI.Util.FromJson<Dictionary<string, string>>(json);
            }
            else
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }
        }

        public static string GetWeaponName(string hash)
        {
            if (WeaponHashToName != null && WeaponHashToName.TryGetValue(hash, out string name))
            {
                return name;
            }
            return "Unknown Weapon";
        }
    }
}
