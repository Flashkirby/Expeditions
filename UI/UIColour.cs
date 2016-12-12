using Microsoft.Xna.Framework;
using Terraria;

namespace Expeditions.UI
{
    public class UIColour
    {
        public readonly static Color backgroundColour = new Color(63, 65, 151, 200);
        public readonly static Color borderColour = new Color(18, 18, 31, 200);
        public readonly static Color Grey = new Color(130, 130, 130);
        public readonly static Color Gray = new Color(130, 130, 130);
        public readonly static Color White = new Color(255, 255, 255);
        public readonly static Color Blue = new Color(150, 150, 255);
        public readonly static Color Green = new Color(150, 255, 150);
        public readonly static Color Orange = new Color(255, 200, 150);
        public readonly static Color LightRed = new Color(255, 150, 150);
        public readonly static Color Pink = new Color(255, 150, 255);
        public readonly static Color LightPurple = new Color(210, 160, 255);
        public readonly static Color Lime = new Color(150, 255, 10);
        public readonly static Color Yellow = new Color(255, 255, 10);
        public readonly static Color Cyan = new Color(5, 200, 255);
        public readonly static Color Red = new Color(255, 40, 100);
        public readonly static Color Purple = new Color(180, 40, 255);
        public readonly static Color Amber = new Color(255, 175, 0);
        public static Color Expert { get { return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 1000); } }

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