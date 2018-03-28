using UnityEngine;
using System;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Translates the object from a point to another using linear translation.
    /// </summary>
    public class LinearMover : LinearPositionTweener
    {
        public void Move(Vector3 from, Vector3 to, float speed, Action<float> updateCallback = null, Action callbackWhenReady = null, Action callbackWhenAborted = null)
        {
            TweenPosition(from, to, speed, GenerateUpdateCallback(updateCallback), callbackWhenReady, callbackWhenAborted);
        }

        public void Move(Vector3 from, Vector3 dir, float speed, float time, Action<float> updateCallback = null, Action callbackWhenReady = null, Action callbackWhenAborted = null)
        {
            TweenPosition(from, dir, speed, time, GenerateUpdateCallback(updateCallback), callbackWhenReady, callbackWhenAborted);
        }

        private Action<float> GenerateUpdateCallback(Action<float> updateCallback)
        {
            return value =>
            {
                transform.position = CurrentPoint;
                if (updateCallback != null)
                {
                    updateCallback(value);
                }
            };
        }
    }
}