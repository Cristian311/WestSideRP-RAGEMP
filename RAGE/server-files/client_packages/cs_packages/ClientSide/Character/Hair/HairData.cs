using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Hair
{
    public class HairData
    {
        [JsonProperty("facialHair")]
        public OtherHairDetail FacialHair { get; set; }

        [JsonProperty("hair")]
        public HairDetail Hair { get; set; }

        [JsonProperty("eyebrows")]
        public OtherHairDetail Eyebrows { get; set; }

        [JsonProperty("bodyHair")]
        public OtherHairDetail BodyHair { get; set; }

        [JsonProperty("hairTattoo")]
        public int HairTattoo { get; set; }
    }

    public class HairDetail
    {
        [JsonProperty("value")]
        public byte Value { get; set; }

        [JsonProperty("color")]
        public byte Color { get; set; }
        [JsonProperty("secondColor")]
        public byte SecondColor { get; set; }

    }
    public class OtherHairDetail
    {
        [JsonProperty("value")]
        public byte Value { get; set; }

        [JsonProperty("color")]
        public byte Color { get; set; }
    }

    public class HairDecal
    {
        public int ID { get; set; }
        public string Collection { get; set; }
        public string Overlay { get; set; }
    }

    public class HairDecalsList
    {
        public List<List<HairDecal>> Decals { get; set; }
    }

}
