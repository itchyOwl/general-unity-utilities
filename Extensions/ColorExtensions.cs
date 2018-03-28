using UnityEngine;
using ItchyOwl.General;

namespace ItchyOwl.Extensions
{
    public static class ColorExtensions
    {
        public static float[] ToArray(this Color color)
        {
            return new float[4] { color.r, color.g, color.b, color.a };
        }

        public static SerializableStructs.Color AsSerializable(this Color color)
        {
            return new SerializableStructs.Color(color.r, color.g, color.b, color.a);
        }

        public static bool EqualsColor(this Color color, Color otherColor, bool ignoreAlpha = true)
        {
            if (ignoreAlpha)
            {
                return Mathf.Approximately(color.r, otherColor.r) && Mathf.Approximately(color.g, otherColor.g) && Mathf.Approximately(color.b, otherColor.b);
            }
            else
            {
                return Mathf.Approximately(color.r, otherColor.r) && Mathf.Approximately(color.g, otherColor.g) && Mathf.Approximately(color.b, otherColor.b) && Mathf.Approximately(color.a, otherColor.a);
            }
        }
    }
}
