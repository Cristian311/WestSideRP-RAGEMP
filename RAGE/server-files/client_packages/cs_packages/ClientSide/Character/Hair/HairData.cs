using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Hair
{
    public class HairData
    {
        [JsonProperty("facialHair")]
        public HairDetail FacialHair { get; set; }

        [JsonProperty("hair")]
        public int Hair { get; set; }

        [JsonProperty("eyebrows")]
        public HairDetail Eyebrows { get; set; }

        [JsonProperty("bodyHair")]
        public HairDetail BodyHair { get; set; }
        [JsonProperty("hairTattoo")]
        public string HairTattoo { get; set; }
    }

    public class HairDetail
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }

        [JsonProperty("secondaryColor")]
        public int SecondaryColor { get; set; }

        [JsonProperty("opacity")]
        public int Opacity { get; set; }
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
