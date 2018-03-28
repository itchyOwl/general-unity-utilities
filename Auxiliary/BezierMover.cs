using UnityEngine;
using System;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Translates the object from a point to another using translation in a Bezier curve.
    /// </summary>
    public class BezierMover : BezierPositionTweener
    {
        public void Move(Vector3 from, Vector3 to, Vector3 controlPoint, float speed, Action<float> updateCallback = null, Action callbackWhenReady = null, Action callbackWhenAborted = null)
        {
            Action<float> newUpdateCallback = value =>
            {
                transform.position = CurrentPoint;
                if (updateCallback != null)
                {
                    updateCallback(value);
                }
            };
            TweenPosition(from, to, controlPoint, speed, newUpdateCallback, callbackWhenReady, callbackWhenAborted);
        }
    }
}