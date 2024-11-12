using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ServerSide.Characters.Clothes
{
    public class ClothesData
    {
        [JsonProperty("mask")]
        public ClothingAttributes Mask { get; set; }

        [JsonProperty("torso")]
        public ClothingAttributes Torso { get; set; }

        [JsonProperty("legs")]
        public ClothingAttributes Legs { get; set; }

        [JsonProperty("bag")]
        public ClothingAttributes Bag { get; set; }

        [JsonProperty("shoes")]
        public ClothingAttributes Shoes { get; set; }

        [JsonProperty("accessories")]
        public ClothingAttributes Accessories { get; set; }

        [JsonProperty("undershirt")]
        public ClothingAttributes Undershirt { get; set; }

        [JsonProperty("bodyArmor")]
        public ClothingAttributes BodyArmor { get; set; }

        [JsonProperty("decals")]
        public ClothingAttributes Decals { get; set; }

        [JsonProperty("tops")]
        public ClothingAttributes Tops { get; set; }

        public class ClothingAttributes
        {
            [JsonProperty("drawable")]
            public int Drawable { get; set; }

            [JsonProperty("texture")]
            public int Texture { get; set; }
        }

        public void PrintProperties()
        {
            PrintProperties(this, 0);
        }

        private void PrintProperties(object obj, int indentLevel)
        {
            if (obj == null)
            {
                NAPI.Util.ConsoleOutput(new string(' ', indentLevel * 2) + "null");
                return;
            }

            Type type = obj.GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                var value = property.GetValue(obj);
                string indent = new string(' ', indentLevel * 2);

                if (value == null)
                {
                    NAPI.Util.ConsoleOutput($"{indent}{property.Name}: null");
                }
                else if (value is string || value.GetType().IsValueType)
                {
                    NAPI.Util.ConsoleOutput($"{indent}{property.Name}: {value}");
                }
                else if (value is Array array)
                {
                    NAPI.Util.ConsoleOutput($"{indent}{property.Name}:");
                    foreach (var item in array)
                    {
                        PrintProperties(item, indentLevel + 1);
                    }
                }
                else
                {
                    NAPI.Util.ConsoleOutput($"{indent}{property.Name}:");
                    PrintProperties(value, indentLevel + 1);
                }
            }
        }
    }
}
