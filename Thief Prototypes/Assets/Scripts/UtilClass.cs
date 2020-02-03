using UnityEngine;
using System.Collections;
using System;

namespace Util
{
    [System.Serializable]
    public class Sound
    {
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = 1;
    }
    public static class UtilClass
    {
        public static Color ChangeColorBrightness(Color color, float brightness)
        {
            float Hue = 0, Saturation = 0, Value = 0;

            Color.RGBToHSV(color, out Hue, out Saturation, out Value);

            color = Color.HSVToRGB(Hue, Saturation, brightness);

            return color;
        }

        public static Color ChangeColorBrightnessSaturation(Color color, float brightness, float saturation)
        {
            float Hue = 0, Saturation = 0, Value = 0;

            Color.RGBToHSV(color, out Hue, out Saturation, out Value);

            color = Color.HSVToRGB(Hue, Saturation * saturation, brightness);

            return color;
        }
    }
}
