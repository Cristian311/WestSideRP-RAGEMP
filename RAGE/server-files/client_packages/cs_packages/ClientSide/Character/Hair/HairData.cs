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
        public string Hair { get; set; }

        [JsonProperty("eyebrows")]
        public HairDetail Eyebrows { get; set; }

        [JsonProperty("bodyHair")]
        public HairDetail BodyHair { get; set; }
    }

    public class HairDetail
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("secondaryColor")]
        public string SecondaryColor { get; set; }

        [JsonProperty("opacity")]
        public string Opacity { get; set; }
    }
}
