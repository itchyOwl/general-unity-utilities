using UnityEngine;
using System;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Calculates translation from a point to another.
    /// Does not move transforms.
    /// Abstract.
    /// </summary>
    public abstract class PositionTweener : Tweener
    {
        public bool useUnscaledTime;
        public Tween.EasingMode easing = Tween.EasingMode.Linear;
        public bool pingPong;
        public float waitBetweenTime;
        public int pingPongCount = 1;

        public Vector3 CurrentPoint { get; private set; }
        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }
        public Vector3 Direction { get { return EndPoint - StartPoint; } }
        public float Distance { get; private set; }
        public float MoveTime { get; private set; }
        public float Speed { get; private set; }

        protected virtual void TweenPositionInternal(Vector3 from, Vector3 to, float speed, Action<float> updateCallback = null, Action callbackWhenReady = null, Action callbackWhenAborted = null)
        {
            StartPoint = from;
            CurrentPoint = StartPoint;
            EndPoint = to;
            Distance = Direction.magnitude;
            Speed = speed;
            MoveTime = Distance / Speed;
            Action<float> newUpdateCallback = value =>
            {
                CurrentPoint = CalculatePosition(value);
                if (updateCallback != null)
                {
                    updateCallback(value);
                }
            };
            callbackWhenReady = callbackWhenReady ?? delegate () { };
            TweenTo(0, Distance, 0, MoveTime, pingPong, waitBetweenTime, pingPongCount, easing, newUpdateCallback, callbackWhenReady, callbackWhenAborted, useUnscaledTime);
        }

        protected abstract Vector3 CalculatePosition(float value);
    }
}