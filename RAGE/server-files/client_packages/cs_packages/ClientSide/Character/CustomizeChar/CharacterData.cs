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
        public float Lips { get; set; }
        [JsonProperty("chin")]
        public ChinAttributes Chin { get; set; }
        [JsonProperty("neck")]
        public NeckAttributes Neck { get; set; }
        [JsonProperty("facialDetails")]
        public FacialAttributes Facial { get; set; }

        public class FacialAttributes
        {
            [JsonProperty("blemishes")]
            public int Blemishes { get; set; }

            [JsonProperty("ageing")]
            public int Ageing { get; set; }

            [JsonProperty("complexion")]
            public int Complexion { get; set; }

            [JsonProperty("sunDamage")]
            public int SunDamage { get; set; }

            [JsonProperty("molesFreckles")]
            public int MolesFreckles { get; set; }
        }
        public class ShapeAttributes
        {
            [JsonProperty("first")]
            public int First { get; set; }

            [JsonProperty("second")]
            public int Second { get; set; }

            [JsonProperty("mix")]
            public float Mix { get; set; }
        }

        public class SkinAttributes
        {
            [JsonProperty("first")]
            public int First { get; set; }

            [JsonProperty("second")]
            public int Second { get; set; }

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

        public class CheeksAtrributes
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
            [JsonProperty("lenght")]
            public float Lenght { get; set; }

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
            public byte Color { get; set; }
        }
        public class NeckAttributes
        {
            [JsonProperty("width")]
            public float Width { get; set; }
        }
        
    }
}
