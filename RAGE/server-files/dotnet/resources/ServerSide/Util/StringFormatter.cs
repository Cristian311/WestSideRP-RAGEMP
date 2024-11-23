using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Util
{
    internal class StringFormatter
    {
        public static string FormatStringForChatMessage(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input; // Return as-is if the input is null or empty
            }

            // Ensure the string starts with a capital letter
            if (!char.IsUpper(input[0]))
            {
                input = char.ToUpper(input[0]) + input.Substring(1);
            } 

            // Ensure the string ends with a period
            if (!input.EndsWith("."))
            {
                input += ".";
            }

            return input;
        }
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
