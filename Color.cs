using System;

namespace GraphDLL
{
    public struct Color
    {
        public byte a;
        public byte r;
        public byte g;
        public byte b;

        public System.Drawing.Color SysDraw { get { return System.Drawing.Color.FromArgb(a, r, g, b); } }


        public static Color FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            Color result = new Color();
            result.a = alpha;
            result.r = red;
            result.g = green;
            result.b = blue;
            return result;
        }

        public static Color FromRGB(byte red, byte green, byte blue)
        {
            Color result = new Color();
            result.a = 255;
            result.r = red;
            result.g = green;
            result.b = blue;
            return result;
        }

        public static bool operator ==(Color a, Color b)
        {
            return (a.r == b.r
                && a.g == b.g
                && a.b == b.b
                && a.a == b.a);
        }

        public static bool operator !=(Color a, Color b)
        {
            return (a.r != b.r
                || a.g != b.g
                || a.b != b.b
                || a.a != b.a);
        }

        public static Color PutAonB(Color a, Color b)
        {
            int back = 255 - a.a;
            return Color.FromARGB(a.a >= b.a ? a.a : b.a
                                , (byte)((a.r * a.a + b.r * back) / 255)
                                , (byte)((a.g * a.a + b.g * back) / 255)
                                , (byte)((a.b * a.a + b.b * back) / 255));
        }


        public override bool Equals(object o)
        {
            try
            {
                return (this == (Color)o);
            }
            catch
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return (a << 32) + (r << 16) + (g << 8) + b;
        }

        public static Color AliceBlue { get { return FromARGB(255, 240, 248, 255); } }
        public static Color AntiqueWhite { get { return FromARGB(255, 250, 235, 215); } }
        public static Color Aqua { get { return FromARGB(255, 0, 255, 255); } }
        public static Color Aquamarine { get { return FromARGB(255, 127, 255, 212); } }
        public static Color Azure { get { return FromARGB(255, 240, 255, 255); } }
        public static Color Beige { get { return FromARGB(255, 245, 245, 220); } }
        public static Color Bisque { get { return FromARGB(255, 255, 228, 196); } }
        public static Color Black { get { return FromARGB(255, 0, 0, 0); } }
        public static Color BlanchedAlmond { get { return FromARGB(255, 255, 235, 205); } }
        public static Color Blue { get { return FromARGB(255, 0, 0, 255); } }
        public static Color BlueViolet { get { return FromARGB(255, 138, 43, 226); } }
        public static Color Brown { get { return FromARGB(255, 165, 42, 42); } }
        public static Color Burlywood { get { return FromARGB(255, 222, 184, 135); } }
        public static Color CadetBlue { get { return FromARGB(255, 95, 158, 160); } }
        public static Color Chartreuse { get { return FromARGB(255, 127, 255, 0); } }
        public static Color Chocolate { get { return FromARGB(255, 210, 105, 30); } }
        public static Color Coral { get { return FromARGB(255, 255, 127, 80); } }
        public static Color CornflowerBlue { get { return FromARGB(255, 100, 149, 237); } }
        public static Color Cornsilk { get { return FromARGB(255, 255, 248, 220); } }
        public static Color Cyan { get { return FromARGB(255, 0, 255, 255); } }
        public static Color DarkBlue { get { return FromARGB(255, 0, 0, 139); } }
        public static Color DarkCyan { get { return FromARGB(255, 0, 139, 139); } }
        public static Color DarkGoldenrod { get { return FromARGB(255, 184, 134, 11); } }
        public static Color DarkGray { get { return FromARGB(255, 169, 169, 169); } }
        public static Color DarkGreen { get { return FromARGB(255, 0, 100, 0); } }
        public static Color DarkKhaki { get { return FromARGB(255, 189, 183, 107); } }
        public static Color DarkMagenta { get { return FromARGB(255, 139, 0, 139); } }
        public static Color DarkOliveGreen { get { return FromARGB(255, 85, 107, 47); } }
        public static Color DarkOrange { get { return FromARGB(255, 255, 140, 0); } }
        public static Color DarkOrchid { get { return FromARGB(255, 153, 50, 204); } }
        public static Color DarkRed { get { return FromARGB(255, 139, 0, 0); } }
        public static Color DarkSalmon { get { return FromARGB(255, 233, 150, 122); } }
        public static Color DarkSeaGreen { get { return FromARGB(255, 143, 188, 143); } }
        public static Color DarkSlateBlue { get { return FromARGB(255, 72, 61, 139); } }
        public static Color DarkSlateGray { get { return FromARGB(255, 47, 79, 79); } }
        public static Color DarkTurquoise { get { return FromARGB(255, 0, 206, 209); } }
        public static Color DarkViolet { get { return FromARGB(255, 148, 0, 211); } }
        public static Color DeepPink { get { return FromARGB(255, 255, 20, 147); } }
        public static Color DeepSkyBlue { get { return FromARGB(255, 0, 191, 255); } }
        public static Color DimGray { get { return FromARGB(255, 105, 105, 105); } }
        public static Color DodgerBlue { get { return FromARGB(255, 30, 144, 255); } }
        public static Color Firebrick { get { return FromARGB(255, 178, 34, 34); } }
        public static Color FloralWhite { get { return FromARGB(255, 255, 250, 240); } }
        public static Color ForestGreen { get { return FromARGB(255, 34, 139, 34); } }
        public static Color Fuschia { get { return FromARGB(255, 255, 0, 255); } }
        public static Color Gainsboro { get { return FromARGB(255, 220, 220, 220); } }
        public static Color GhostWhite { get { return FromARGB(255, 255, 250, 250); } }
        public static Color Gold { get { return FromARGB(255, 255, 215, 0); } }
        public static Color Goldenrod { get { return FromARGB(255, 218, 165, 32); } }
        public static Color Gray { get { return FromARGB(255, 128, 128, 128); } }
        public static Color Green { get { return FromARGB(255, 0, 128, 0); } }
        public static Color GreenYellow { get { return FromARGB(255, 173, 255, 47); } }
        public static Color Honeydew { get { return FromARGB(255, 240, 255, 240); } }
        public static Color HotPink { get { return FromARGB(255, 255, 105, 180); } }
        public static Color IndianRed { get { return FromARGB(255, 205, 92, 92); } }
        public static Color Ivory { get { return FromARGB(255, 255, 255, 240); } }
        public static Color Khaki { get { return FromARGB(255, 240, 230, 140); } }
        public static Color Lavender { get { return FromARGB(255, 230, 230, 250); } }
        public static Color LavenderBlush { get { return FromARGB(255, 255, 240, 245); } }
        public static Color LawnGreen { get { return FromARGB(255, 124, 252, 0); } }
        public static Color LemonChiffon { get { return FromARGB(255, 255, 250, 205); } }
        public static Color LightBlue { get { return FromARGB(255, 173, 216, 230); } }
        public static Color LightCoral { get { return FromARGB(255, 240, 128, 128); } }
        public static Color LightCyan { get { return FromARGB(255, 224, 255, 255); } }
        public static Color LightGoldenrod { get { return FromARGB(255, 238, 221, 130); } }
        public static Color LightGoldenrodYellow { get { return FromARGB(255, 250, 250, 210); } }
        public static Color LightGray { get { return FromARGB(255, 211, 211, 211); } }
        public static Color LightGreen { get { return FromARGB(255, 144, 238, 144); } }
        public static Color LightPink { get { return FromARGB(255, 255, 182, 193); } }
        public static Color LightSalmon { get { return FromARGB(255, 255, 160, 122); } }
        public static Color LightSeaGreen { get { return FromARGB(255, 32, 178, 170); } }
        public static Color LightSkyBlue { get { return FromARGB(255, 135, 206, 250); } }
        public static Color LightSlateBlue { get { return FromARGB(255, 132, 112, 255); } }
        public static Color LightSlateGray { get { return FromARGB(255, 119, 136, 153); } }
        public static Color LightSteelBlue { get { return FromARGB(255, 176, 196, 222); } }
        public static Color LightYellow { get { return FromARGB(255, 255, 255, 224); } }
        public static Color Lime { get { return FromARGB(255, 0, 255, 0); } }
        public static Color LimeGreen { get { return FromARGB(255, 50, 205, 50); } }
        public static Color Linen { get { return FromARGB(255, 250, 240, 230); } }
        public static Color Magenta { get { return FromARGB(255, 255, 0, 255); } }
        public static Color Maroon { get { return FromARGB(255, 128, 0, 0); } }
        public static Color MediumAquamarine { get { return FromARGB(255, 102, 205, 170); } }
        public static Color MediumBlue { get { return FromARGB(255, 0, 0, 205); } }
        public static Color MediumOrchid { get { return FromARGB(255, 186, 85, 211); } }
        public static Color MediumPurple { get { return FromARGB(255, 147, 112, 219); } }
        public static Color MediumSeaGreen { get { return FromARGB(255, 60, 179, 113); } }
        public static Color MediumSlateBlue { get { return FromARGB(255, 123, 104, 238); } }
        public static Color MediumSpringGreen { get { return FromARGB(255, 0, 250, 154); } }
        public static Color MediumTurquoise { get { return FromARGB(255, 72, 209, 204); } }
        public static Color MediumVioletRed { get { return FromARGB(255, 199, 21, 133); } }
        public static Color MidnightBlue { get { return FromARGB(255, 25, 25, 112); } }
        public static Color MintCream { get { return FromARGB(255, 245, 255, 250); } }
        public static Color MistyRose { get { return FromARGB(255, 255, 228, 225); } }
        public static Color Moccasin { get { return FromARGB(255, 255, 228, 181); } }
        public static Color NavajoWhite { get { return FromARGB(255, 255, 222, 173); } }
        public static Color Navy { get { return FromARGB(255, 0, 0, 128); } }
        public static Color OldLace { get { return FromARGB(255, 253, 245, 230); } }
        public static Color Olive { get { return FromARGB(255, 128, 128, 0); } }
        public static Color OliveDrab { get { return FromARGB(255, 107, 142, 35); } }
        public static Color Orange { get { return FromARGB(255, 255, 165, 0); } }
        public static Color OrangeRed { get { return FromARGB(255, 255, 69, 0); } }
        public static Color Orchid { get { return FromARGB(255, 218, 112, 214); } }
        public static Color PaleGoldenrod { get { return FromARGB(255, 238, 232, 170); } }
        public static Color PaleGreen { get { return FromARGB(255, 152, 251, 152); } }
        public static Color PaleTurquoise { get { return FromARGB(255, 175, 238, 238); } }
        public static Color PaleVioletRed { get { return FromARGB(255, 219, 112, 147); } }
        public static Color PapayaWhip { get { return FromARGB(255, 255, 239, 213); } }
        public static Color PeachPuff { get { return FromARGB(255, 255, 218, 185); } }
        public static Color Peru { get { return FromARGB(255, 205, 133, 63); } }
        public static Color Pink { get { return FromARGB(255, 255, 192, 203); } }
        public static Color Plum { get { return FromARGB(255, 221, 160, 221); } }
        public static Color PowderBlue { get { return FromARGB(255, 176, 224, 230); } }
        public static Color Purple { get { return FromARGB(255, 128, 0, 128); } }
        public static Color Red { get { return FromARGB(255, 255, 0, 0); } }
        public static Color RosyBrown { get { return FromARGB(255, 188, 143, 143); } }
        public static Color RoyalBlue { get { return FromARGB(255, 65, 105, 225); } }
        public static Color SaddleBrown { get { return FromARGB(255, 139, 69, 19); } }
        public static Color Salmon { get { return FromARGB(255, 250, 128, 114); } }
        public static Color SandyBrown { get { return FromARGB(255, 244, 164, 96); } }
        public static Color SeaGreen { get { return FromARGB(255, 46, 139, 87); } }
        public static Color Seashell { get { return FromARGB(255, 255, 245, 238); } }
        public static Color Sienna { get { return FromARGB(255, 160, 82, 45); } }
        public static Color Silver { get { return FromARGB(255, 192, 192, 192); } }
        public static Color SkyBlue { get { return FromARGB(255, 135, 206, 235); } }
        public static Color SlateBlue { get { return FromARGB(255, 106, 90, 205); } }
        public static Color SlateGray { get { return FromARGB(255, 112, 128, 144); } }
        public static Color Snow { get { return FromARGB(255, 255, 250, 250); } }
        public static Color SpringGreen { get { return FromARGB(255, 0, 255, 127); } }
        public static Color SteelBlue { get { return FromARGB(255, 70, 130, 180); } }
        public static Color Tan { get { return FromARGB(255, 210, 180, 140); } }
        public static Color Teal { get { return FromARGB(255, 0, 128, 128); } }
        public static Color Thistle { get { return FromARGB(255, 216, 191, 216); } }
        public static Color Tomato { get { return FromARGB(255, 255, 99, 71); } }
        public static Color Turquoise { get { return FromARGB(255, 64, 224, 208); } }
        public static Color Violet { get { return FromARGB(255, 238, 130, 238); } }
        public static Color VioletRed { get { return FromARGB(255, 208, 32, 144); } }
        public static Color Wheat { get { return FromARGB(255, 245, 222, 179); } }
        public static Color White { get { return FromARGB(255, 255, 255, 255); } }
        public static Color WhiteSmoke { get { return FromARGB(255, 245, 245, 245); } }
        public static Color Yellow { get { return FromARGB(255, 255, 255, 0); } }
        public static Color YellowGreen { get { return FromARGB(255, 154, 205, 50); } }

    }
}
