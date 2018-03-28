using UnityEngine;
using System;

namespace ItchyOwl.Extensions
{
    public static class ArrayExtensions
    {
        public static Color ToRGBA(this float[] floatArray)
        {
            if (floatArray == null) { throw new Exception("[ArrayExtensions] The array is null."); }
            if (floatArray.Length < 4) { throw new Exception("[ArrayExtensions] The array is too short to convert into a color."); }
            return new Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
        }
    }
}
