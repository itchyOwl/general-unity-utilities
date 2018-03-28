using UnityEngine;
using System;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Calculates translation from a point to another using linear translation.
    /// Does not move transforms.
    /// </summary>
    public class LinearPositionTweener : PositionTweener
    {
        public void TweenPosition(Vector3 from, Vector3 to, float speed, Action<float> updateCallback = null, Action callbackWhenReady = null, Action callbackWhenAborted = null)
        {
            TweenPositionInternal(from, to, speed, updateCallback, callbackWhenReady, callbackWhenAborted);
        }

        public void TweenPosition(Vector3 from, Vector3 dir, float speed, float time, Action<float> updateCallback = null, Action callbackWhenReady = null, Action callbackWhenAborted = null)
        {
            Vector3 to = from + (dir.normalized * speed * time);
            TweenPositionInternal(from, to, speed, updateCallback, callbackWhenReady, callbackWhenAborted);
        }

        protected override Vector3 CalculatePosition(float value)
        {
            return Vector3.Lerp(StartPoint, EndPoint, value / Distance);
        }
    }
}