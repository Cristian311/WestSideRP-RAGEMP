using Newtonsoft.Json;

namespace ClientSide.Character.Clothes
{
    public class ClothesData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("outfitName")]
        public string Name { get; set; }

        [JsonProperty("masks")]
        public ClothingAttributes Mask { get; set; }

        [JsonProperty("torsos")]
        public ClothingAttributes Torso { get; set; }

        [JsonProperty("legs")]
        public ClothingAttributes Legs { get; set; }

        [JsonProperty("bags")]
        public ClothingAttributes Bag { get; set; }

        [JsonProperty("shoes")]
        public ClothingAttributes Shoes { get; set; }

        [JsonProperty("accessories")]
        public ClothingAttributes Accessories { get; set; }

        [JsonProperty("undershirts")]
        public ClothingAttributes Undershirt { get; set; }

        [JsonProperty("bodyArmor")]
        public ClothingAttributes BodyArmor { get; set; }

        [JsonProperty("decals")]
        public ClothingAttributes Decals { get; set; }

        [JsonProperty("shirts")]
        public ClothingAttributes Shirts { get; set; }
        [JsonProperty("helmets")]
        public ClothingAttributes Helmets { get; set; }
        [JsonProperty("glasses")]
        public ClothingAttributes Glasses { get; set; }
        [JsonProperty("earAccesories")]
        public ClothingAttributes EarAccesories { get; set; }
        [JsonProperty("watches")]
        public ClothingAttributes Watches { get; set; }
        [JsonProperty("bracelets")]
        public ClothingAttributes Bracelets { get; set; }

        public class ClothingAttributes
        {
            [JsonProperty("drawable")]
            public int Drawable { get; set; }

            [JsonProperty("texture")]
            public int Texture { get; set; }
        }


    }
}
