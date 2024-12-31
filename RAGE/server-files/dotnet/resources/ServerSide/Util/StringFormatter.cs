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
                return input; 
            }

            if (!char.IsUpper(input[0]))
            {
                input = char.ToUpper(input[0]) + input.Substring(1);
            } 

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
        public static string FormatStringForAme(string name, string action)
        {
            const int maxLineLength = 70;
            string prefix = $"~p~{name} {action}";
            string suffix = $"~w~{name}";

            List<string> parts = new List<string>();

            while (prefix.Length > 0)
            {
                if (prefix.Length <= maxLineLength)
                {
                    // If the remaining part fits within the limit, add it as is.
                    parts.Add(prefix);
                    break;
                }

                // Find the last space within the allowed length to avoid splitting words.
                int splitIndex = prefix.LastIndexOf(' ', maxLineLength);
                if (splitIndex == -1)
                {
                    // If no space is found (unlikely in normal text), force a hard split.
                    splitIndex = maxLineLength;
                }

                // Add the part up to the split point and trim the processed part.
                parts.Add(prefix.Substring(0, splitIndex));
                prefix = prefix.Substring(splitIndex).TrimStart();
            }

            // Join the parts with newlines and add the suffix at the end.
            return string.Join("\n", parts) + $"\n{suffix}";
        }

    }
}
