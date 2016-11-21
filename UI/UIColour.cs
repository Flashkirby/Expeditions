using Microsoft.Xna.Framework;
using Terraria;

namespace Expeditions.UI
{
    internal class UIColour
    {
        public readonly static Color backgroundColour = new Color(63, 65, 151, 200);
        public readonly static Color borderColour = new Color(18, 18, 31, 200);
        public readonly static Color Gray = new Color(130, 130, 130);
        public readonly static Color White = new Color(130, 130, 130);
        public readonly static Color Blue = new Color(130, 130, 130);
        public readonly static Color Green = new Color(130, 130, 130);
        public readonly static Color Orange = new Color(130, 130, 130);
        public readonly static Color LightRed = new Color(130, 130, 130);
        public readonly static Color Pink = new Color(130, 130, 130);
        public readonly static Color LightPurple = new Color(130, 130, 130);
        public readonly static Color Lime = new Color(130, 130, 130);
        public readonly static Color Yellow = new Color(130, 130, 130);
        public readonly static Color Cyan = new Color(130, 130, 130);
        public readonly static Color Red = new Color(130, 130, 130);
        public readonly static Color Purple = new Color(130, 130, 130);
        public readonly static Color Amber = new Color(130, 130, 130);
        public static Color Expert { get { return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB); } }

        public static Color GetColourFromRarity(int rarity)
        {
            switch(rarity)
            {
                case -12: return Expert;
                case -11: return Amber;
                case 1: return Blue;
                case 2: return Green;
                case 3: return Orange;
                case 4: return LightRed;
                case 5: return Pink;
                case 6: return LightPurple;
                case 7: return Lime;
                case 8: return Yellow;
                case 9: return Cyan;
                case 10: return Red;
                case 11: return Purple;
                default: return White;
            }
        }
    }
}