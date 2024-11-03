using MySql.Data.MySqlClient;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters
{
    internal class CharacterManager
    {
        public void getNamesAndMoneyForCEF(List<ICharacter> characters, int requiredCount, out string[] names, out int[] money)
        {
            names = new string[requiredCount];
            money = new int[requiredCount];
            if (characters != null)
            {
                for (int i = 0; i < requiredCount; i++)
                {
                    if (characters[i] != null)
                    {
                        names[i] = characters[i].Name;
                        money[i] = characters[i].Money;
                    }
                    else
                    {
                        names[i] = "none";
                        money[i] = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < requiredCount; i++)
                {
                    names[i] = "none";
                    money[i] = 0;
                }    
            }
        }
        public int getRequiredCount(List<ICharacter> characters)
        {
            int requiredCount;
            if (characters == null)
                requiredCount = 3;
            else
                requiredCount = Math.Max(3, characters.Count);
            return requiredCount;
        }
    }
}
