namespace URSA.Utility {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public static class ColorUtilities {
        public static Color SetAlpha(this Color color, float a) {
            return new Color(color.r, color.g, color.b, a);
        }

        public static Color HexToColor(this string hex) {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8) {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static string ToHex(this Color color) {
            Color32 col = color;
            string hex = col.r.ToString("X2") + col.g.ToString("X2") + col.b.ToString("X2");
            return hex;
        }
    }

}