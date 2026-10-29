using System.Collections.Generic;

namespace ServerSide.Server.Items.Weapons
{
    internal class WeaponData
    {
        public static List<string> WhiteWeaponData = new List<string>
        {
            "ID: weapon_dagger Name: Antique Cavalry Dagger Hash: 0x92A27487",
            "ID: weapon_bat Name: Baseball Bat Hash: 0x958A4A8F",
            "ID: weapon_bottle Name: Broken Bottle Hash: 0xF9E6AA4B",
            "ID: weapon_crowbar Name: Crowbar Hash: 0x84BD7BFD",
            "ID: weapon_unarmed Name: Fist Hash: 0xA2719263",
            "ID: weapon_flashlight Name: Flashlight Hash: 0x8BB05FD7",
            "ID: weapon_golfclub Name: Golf Club Hash: 0x440E4788",
            "ID: weapon_hammer Name: Hammer Hash: 0x4E875F73",
            "ID: weapon_hatchet Name: Hatchet Hash: 0xF9DCBF2D",
            "ID: weapon_knuckle Name: Brass Knuckles Hash: 0xD8DF3C3C",
            "ID: weapon_knife Name: Knife Hash: 0x99B507EA",
            "ID: weapon_machete Name: Machete Hash: 0xDD5DF8D9",
            "ID: weapon_switchblade Name: Switchblade Hash: 0xDFE37640",
            "ID: weapon_nightstick Name: Nightstick Hash: 0x678B81B1",
            "ID: weapon_wrench Name: Pipe Wrench Hash: 0x19044EE0",
            "ID: weapon_battleaxe Name: Battle Axe Hash: 0xCD274149",
            "ID: weapon_poolcue Name: Pool Cue Hash: 0x94117305",
            "ID: weapon_stone_hatchet Name: Stone Hatchet Hash: 0x3813FC08",
            "ID: weapon_candycane Name: Candy Cane Hash: 0x6589186A"
        };



        public static List<string> ExtractWeaponNames(List<string> data)
        {
            var ids = new List<string>();

            foreach (var line in data)
            {
                // Find the part of the string that contains the ID
                int idStart = line.IndexOf("ID: ") + 4; // Start of ID
                int idEnd = line.IndexOf(" ", idStart); // End of ID (space after ID)

                if (idStart != -1 && idEnd != -1)
                {
                    string id = line.Substring(idStart, idEnd - idStart);
                    ids.Add(id);
                }
            }

            return ids;
        }
    }
}

