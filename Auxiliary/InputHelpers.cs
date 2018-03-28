using System.Collections.Generic;
using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public static class InputHelpers
    {
        #region SmoothDamp
        private class Damp
        {
            public float smoothTime;
            public float velocity;
            public float current;
        }

        private static Dictionary<string, Damp> damps = new Dictionary<string, Damp>();

        private static Damp GetDamp(string axisName, float smoothTime)
        {
            Damp damp;
            if (!damps.TryGetValue(axisName, out damp))
            {
                damp = new Damp() { smoothTime = smoothTime };
                damps.Add(axisName, damp);
            }
            return damp;
        }
        #endregion

        /// <summary>
        /// Smoothes the raw input (which must be used if you use unscaled time) with Mathf.SmoothDamp.
        /// </summary>
        public static float GetAxisRawSmooth(string axisName, float deltaTime, float smoothTime = 0.1f)
        {
            var damp = GetDamp(axisName, smoothTime);
            damp.current = Mathf.SmoothDamp(damp.current, Input.GetAxisRaw(axisName), ref damp.velocity, damp.smoothTime, Mathf.Infinity, deltaTime);
            return damp.current;
        }
    }
}
