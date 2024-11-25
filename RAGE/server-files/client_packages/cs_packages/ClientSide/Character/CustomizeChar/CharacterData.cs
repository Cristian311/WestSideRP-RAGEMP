using Newtonsoft.Json;
using System.Reflection;
using System; 

namespace Clientside.CustomizeChar
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
        public CheeksAtrributes Cheeks { get; set; }

        [JsonProperty("jaw")]
        public JawAttributes Jaw { get; set; }

        [JsonProperty("skinColor")]
        public string SkinColor { get; set; }

        [JsonProperty("eye")]
        public EyeAttributes Eye { get; set; }
        [JsonProperty("lips")]
        public string Lips { get; set; }
        [JsonProperty("chin")]
        public ChinAttributes Chin { get; set; }
        [JsonProperty("neck")]
        public NeckAttributes Neck { get; set; }

        public class ShapeAttributes
        {
            [JsonProperty("first")]
            public string First { get; set; }

            [JsonProperty("second")]
            public string Second { get; set; }

            [JsonProperty("mix")]
            public string Mix { get; set; }
        }

        public class SkinAttributes
        {
            [JsonProperty("first")]
            public string First { get; set; }

            [JsonProperty("second")]
            public string Second { get; set; }

            [JsonProperty("mix")]
            public string Mix { get; set; }
        }

        public class NoseAttributes
        {
            [JsonProperty("width")]
            public string Width { get; set; }

            [JsonProperty("height")]
            public string Height { get; set; }

            [JsonProperty("length")]
            public string Length { get; set; }

            [JsonProperty("bridge")]
            public string Bridge { get; set; }

            [JsonProperty("tip")]
            public string Tip { get; set; }

            [JsonProperty("bridgeShift")]
            public string BridgeShift { get; set; }
        }

        public class BrowAttributes
        {
            [JsonProperty("height")]
            public string Height { get; set; }

            [JsonProperty("width")]
            public string Width { get; set; }
        }

        public class CheeksAtrributes
        {
            [JsonProperty("height")]
            public string Height { get; set; }
            [JsonProperty("width")]
            public string Width { get; set; }
            [JsonProperty("cheekswidth")]
            public string CheeksWidth { get; set; }
        }
        public class JawAttributes
        {
            [JsonProperty("width")]
            public string Width { get; set; }
        }
        public class ChinAttributes
        {
            [JsonProperty("lenght")]
            public string Lenght { get; set; }

            [JsonProperty("position")]
            public string Position { get; set; }
            [JsonProperty("width")]
            public string Width { get; set; }
            [JsonProperty("shape")]
            public string Shape { get; set; }
        }
        public class EyeAttributes
        {
            [JsonProperty("size")]
            public string Size { get; set; }

            [JsonProperty("color")]
            public string Color { get; set; }
        }
        public class NeckAttributes
        {
            [JsonProperty("width")]
            public string Width { get; set; }
        }
        
    }
}
