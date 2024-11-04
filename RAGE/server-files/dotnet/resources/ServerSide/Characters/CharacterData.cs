using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters
{
    public class CharacterData
    {
        public ShapeAttributes Shape { get; set; }
        public SkinAttributes Skin { get; set; }
        public NoseAttributes Nose { get; set; }
        public BrowAttributes Brow { get; set; }
        public JawAttributes Jaw { get; set; }
        public string SkinColor { get; set; }
        public EyeAttributes Eye { get; set; }

        public class ShapeAttributes
        {
            public string First { get; set; }
            public string Second { get; set; }
            public string Third { get; set; }
            public string Mix { get; set; }
        }

        public class SkinAttributes
        {
            public string First { get; set; }
            public string Second { get; set; }
            public string Third { get; set; }
            public string Mix { get; set; }
        }

        public class NoseAttributes
        {
            public string Width { get; set; }
            public string Height { get; set; }
            public string Length { get; set; }
            public string Bridge { get; set; }
            public string Tip { get; set; }
            public string BridgeShift { get; set; }
        }

        public class BrowAttributes
        {
            public string Height { get; set; }
            public string Width { get; set; }
        }

        public class JawAttributes
        {
            public string Width { get; set; }
        }

        public class EyeAttributes
        {
            public string Size { get; set; }
            public string Color { get; set; }
        }
    }

}
