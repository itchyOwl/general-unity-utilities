using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ItchyOwl.General;
using ItchyOwl.Extensions;
using System.Linq;
using Math = ItchyOwl.General.Math;

namespace ItchyOwl.Auxiliary
{
    public class Tween
    {
        public enum EasingMode
        {
            Linear,
            EaseIn,
            EaseOut,
            Smooth,
            Smoother,
            Exponential
        }
        public bool useUnscaledTime;
        public EasingMode easing;
        public int tweenId;
        public Coroutine coroutine;
        public float from;
        public float to;
        public bool pingPong;
        public int pingPongCount;
        public float waitBetweenTime;
        public float currentValue;
        public float duration;
        public float startTime;
        public float endTime;
        public float currentTime;
        public float lerpTime;
        public bool isRunning;
        public Action<float> updateCallback;
        public Action readyCallback;
        public Action abortCallback;

        public delegate void TweenUpdate(Tween tween, float value);
        public static event TweenUpdate ValueUpdated;

        public void UpdateValue(float value)
        {
            currentValue = value;
            updateCallback(value);
            if (ValueUpdated != null)
            {
                ValueUpdated(this, value);
            }
        }

        public void Abort()
        {
            isRunning = false;
            if (abortCallback != null) { abortCallback(); }
        }
    }

    /// <summary>
    /// This class can tween any float value. It accepts callbacks and sends events when the value is updated and when the routine is ready.
    /// Tweener provides two events: TweenStarted and TweenReady, Tween provides one event: ValueUpdated. All the events are static and all provide references to the tween instance as event arguments.
    /// TODO: Give an animation curve as the easing mode? vrt. BezierPositionTweener
    /// </summary>
    public class Tweener : MonoBehaviour
    {
        public bool IsRunning { get { return tweens.Values.Any(tween => tween.isRunning); } }

        public static event Action<Tween> TweenStarted;
        public static event Action<Tween> TweenReady;
        public static event Action<Tween> TweenAborted;

        private Dictionary<int, Tween> tweens = new Dictionary<int, Tween>();
        public Dictionary<int, Tween> Tweens { get { return tweens; } }

        /// <summary>
        /// Id can be any integer that is not yet used by this Tweener. If a used id is provided, the old tween will be aborted and replaced with the new tween.
        /// From value is optional. If it is not provided, the latest value of the current tween is used as the start value.
        /// </summary>
        public void TweenTo(int tweenId, float to, float? from = null, float duration = 1, bool pingPong = false, float waitBetweenTime = 0, int pingPongCount = 1, Tween.EasingMode easing = Tween.EasingMode.Linear, Action<float> updateCallback = null, Action readyCallback = null, Action abortCallback = null, bool useUnscaledTime = false)
        {
            Tween tween;
            if (tweens.TryGetValue(tweenId, out tween))
            {
                StopTween(tween);
            }
            else
            {
                tween = new Tween();
                tweens.Add(tweenId, tween);
                //Debug.LogFormat("[Tweener] tween added with the id {0}", tweenId);
            }
            from = from ?? tween.currentValue;   // If the start value is not defined, we will use the current value. If the id matches another tween, the value is inherited from it.
            SetupTween(tween, tweenId, from.Value, to, duration, pingPong, waitBetweenTime, pingPongCount, easing, updateCallback, readyCallback, abortCallback, useUnscaledTime);
        }

        public void StopAll()
        {
            if (IsRunning)
            {
                tweens.ForEach(tween => StopTween(tween.Value));
            }
        }

        private void StopTween(Tween tween)
        {
            if (tween.isRunning)
            {
                tween.Abort();
                if (TweenAborted != null)
                {
                    TweenAborted(tween);
                }
            }
            if (tween.coroutine != null)
            {
                StopCoroutine(tween.coroutine);
            }
            // Note: do not remove completed tweens, because we lose data (esp. currentValue)
        }

        private void SetupTween(Tween tween, int id, float from, float to, float duration, bool pingPong, float waitBetweenTime, int pingPontCount, Tween.EasingMode easing, Action<float> updateCallback, Action readyCallback, Action abortCallback, bool useUnscaledTime)
        {
            tween.tweenId = id;
            tween.startTime = Time.timeSinceLevelLoad;
            tween.duration = duration;
            tween.endTime = tween.startTime + tween.duration;
            tween.from = from;
            tween.to = to;
            tween.pingPong = pingPong;
            tween.waitBetweenTime = waitBetweenTime;
            tween.pingPongCount = pingPontCount;
            tween.easing = easing;
            tween.useUnscaledTime = useUnscaledTime;
            tween.updateCallback = updateCallback ?? delegate { };
            tween.readyCallback = readyCallback ?? delegate { };
            tween.abortCallback = abortCallback ?? delegate { };
            tween.UpdateValue(tween.from);
            if (gameObject.activeInHierarchy)
            {
                tween.coroutine = StartCoroutine(TweeningRoutine(tween));
            }
            else
            {
                Debug.LogWarning("[Tweener] " + name + " is not active, aborting the tween.");
                StopTween(tween);
            }
        }

        private IEnumerator TweeningRoutine(Tween tween)
        {
            tween.isRunning = true;
            var delay = new WaitForSeconds(tween.waitBetweenTime);
            var unscaledDelay = new WaitForSecondsRealtime(tween.waitBetweenTime);
            if (TweenStarted != null)
            {
                TweenStarted(tween);
            }
            while (true)
            {
                // TODO: this is scaled time? -> the logic is not right for unscaled time?
                if (Time.timeSinceLevelLoad >= tween.endTime)
                {
                    tween.UpdateValue(tween.to);
                    if (tween.pingPong && tween.pingPongCount > 0)
                    {
                        if (tween.useUnscaledTime)
                        {
                            yield return unscaledDelay;
                        }
                        else
                        {
                            yield return delay;
                        }
                        float previousTarget = tween.to;
                        tween.to = tween.from;
                        tween.from = previousTarget;
                        tween.startTime = Time.timeSinceLevelLoad;
                        tween.endTime = tween.startTime + tween.duration;
                        tween.pingPongCount--;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    tween.lerpTime = Mathf.InverseLerp(tween.startTime, tween.endTime, Time.timeSinceLevelLoad);
                    float lerp = 0;
                    switch (tween.easing)
                    {
                        case Tween.EasingMode.Linear:
                            lerp = Mathf.Lerp(tween.from, tween.to, tween.lerpTime);
                            break;
                        case Tween.EasingMode.EaseIn:
                            lerp = Mathf.Lerp(tween.from, tween.to, Math.EaseIn(tween.lerpTime));
                            break;
                        case Tween.EasingMode.EaseOut:
                            lerp = Mathf.Lerp(tween.from, tween.to, Math.EaseOut(tween.lerpTime));
                            break;
                        case Tween.EasingMode.Exponential:
                            lerp = Mathf.Lerp(tween.from, tween.to, Math.Exponential(tween.lerpTime));
                            break;
                        case Tween.EasingMode.Smooth:
                            lerp = Mathf.Lerp(tween.from, tween.to, Math.SmoothStep(tween.lerpTime));
                            break;
                        case Tween.EasingMode.Smoother:
                            lerp = Mathf.Lerp(tween.from, tween.to, Math.SmootherStep(tween.lerpTime));
                            break;
                        default: throw new NotImplementedException(tween.easing.ToString());
                    }
                    tween.UpdateValue(lerp);
                }
                yield return null;
            }
            tween.isRunning = false;
            tween.readyCallback();
            if (TweenReady != null)
            {
                TweenReady(tween);
            }
            yield return null;  // Ensures that we don't receive null as the coroutine.
            // Note: do not remove completed tweens, because we lose data (esp. currentValue)
        }
    }
}
