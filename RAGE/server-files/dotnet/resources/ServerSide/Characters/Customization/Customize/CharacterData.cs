using Newtonsoft.Json;
using GTANetworkAPI;
using System.Reflection;
using System;

namespace ServerSide.Characters.Customization.Customize
{
    public class CharacterData
    {
        [JsonProperty("shape")]
        public ShapeAttributes Shape { get; set; }

        [JsonProperty("skin")]
        public SkinAttributes Skin { get; set; }

        [JsonProperty("nose")]
        public NoseAttributes Nose { get; set; }

        [JsonProperty("brow")]
        public BrowAttributes Brow { get; set; }

        [JsonProperty("cheeks")]
        public CheeksAttributes Cheeks { get; set; }

        [JsonProperty("jaw")]
        public JawAttributes Jaw { get; set; }

        [JsonProperty("skinColor")]
        public string SkinColor { get; set; } // Still keeping as string if it's necessary for skin color code (hex or name).

        [JsonProperty("eye")]
        public EyeAttributes Eye { get; set; }

        [JsonProperty("lips")]
        public float Lips { get; set; } // Change string to float for easier use.

        [JsonProperty("chin")]
        public ChinAttributes Chin { get; set; }

        [JsonProperty("neck")]
        public NeckAttributes Neck { get; set; }

        [JsonProperty("facialDetails")]
        public FacialAttributes Facial { get; set; }

        public class FacialAttributes
        {
            [JsonProperty("blemishes")]
            public byte Blemishes { get; set; }

            [JsonProperty("ageing")]
            public byte Ageing { get; set; }

            [JsonProperty("complexion")]
            public byte Complexion { get; set; }

            [JsonProperty("sunDamage")]
            public byte SunDamage { get; set; }

            [JsonProperty("molesFreckles")]
            public byte MolesFreckles { get; set; }
        }

        public class ShapeAttributes
        {
            [JsonProperty("first")]
            public byte First { get; set; } 

            [JsonProperty("second")]
            public byte Second { get; set; } 

            [JsonProperty("mix")]
            public float Mix { get; set; } 
        }

        public class SkinAttributes
        {
            [JsonProperty("first")]
            public byte First { get; set; } 

            [JsonProperty("second")]
            public byte Second { get; set; } 

            [JsonProperty("mix")]
            public float Mix { get; set; } 
        }

        public class NoseAttributes
        {
            [JsonProperty("width")]
            public float Width { get; set; } 

            [JsonProperty("height")]
            public float Height { get; set; } 

            [JsonProperty("length")]
            public float Length { get; set; } 

            [JsonProperty("bridge")]
            public float Bridge { get; set; } 

            [JsonProperty("tip")]
            public float Tip { get; set; } 

            [JsonProperty("bridgeShift")]
            public float BridgeShift { get; set; } 
        }

        public class BrowAttributes
        {
            [JsonProperty("height")]
            public float Height { get; set; } 

            [JsonProperty("width")]
            public float Width { get; set; } 
        }

        public class CheeksAttributes
        {
            [JsonProperty("height")]
            public float Height { get; set; } 

            [JsonProperty("width")]
            public float Width { get; set; } 

            [JsonProperty("cheekswidth")]
            public float CheeksWidth { get; set; } 
        }

        public class JawAttributes
        {
            [JsonProperty("height")]
            public float Height { get; set; }

            [JsonProperty("width")]
            public float Width { get; set; } 
        }

        public class ChinAttributes
        {
            [JsonProperty("length")]
            public float Length { get; set; } 

            [JsonProperty("position")]
            public float Position { get; set; } 

            [JsonProperty("width")]
            public float Width { get; set; } 

            [JsonProperty("shape")]
            public float Shape { get; set; } 
        }

        public class EyeAttributes
        {
            [JsonProperty("size")]
            public float Size { get; set; } 

            [JsonProperty("color")]
            public byte Color { get; set; } // If it's a color code or name, keep it as string.
        }

        public class NeckAttributes
        {
            [JsonProperty("width")]
            public float Width { get; set; } 
        }
        public void PrintProperties()
        {
            PrintProperties(this, 0);
        }

        private void PrintProperties(object obj, int indentLevel)
        {
            // Check if the object itself is null
            if (obj == null)
            {
                NAPI.Util.ConsoleOutput(new string(' ', indentLevel * 2) + "null");
                return;
            }

            Type type = obj.GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                var value = property.GetValue(obj);
                string indent = new string(' ', indentLevel * 2); // Increase indentation for nested properties

                if (value == null) // Check if the property value is null
                {
                    NAPI.Util.ConsoleOutput($"{indent}{property.Name}: null");
                }
                else if (value is string || value.GetType().IsValueType) // Handle strings and value types
                {
                    NAPI.Util.ConsoleOutput($"{indent}{property.Name}: {value}");
                }
                else if (value is Array array) // Handle arrays
                {
                    NAPI.Util.ConsoleOutput($"{indent}{property.Name}:");
                    foreach (var item in array)
                    {
                        PrintProperties(item, indentLevel + 1); // Recursive call for array items
                    }
                }
                else // Handle complex types (nested objects)
                {
                    NAPI.Util.ConsoleOutput($"{indent}{property.Name}:");
                    PrintProperties(value, indentLevel + 1); // Recursive call for nested properties
                }
            }
        }

    }

    
}
