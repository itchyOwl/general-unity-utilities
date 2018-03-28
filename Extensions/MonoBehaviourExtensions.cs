using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ItchyOwl.Extensions
{
    /// <summary>
    /// Can be used to pair up a coroutine and the owner.
    /// </summary>
    public class CoroutineWrapper
    {
        public readonly Coroutine coroutine;
        public readonly MonoBehaviour owner;

        public CoroutineWrapper(Coroutine coroutine, MonoBehaviour owner)
        {
            this.coroutine = coroutine;
            this.owner = owner;
        }
    }

    public static class MonoBehaviorExtensions
    {
        private static Dictionary<GameObject, List<CoroutineWrapper>> delayedSetActives = new Dictionary<GameObject, List<CoroutineWrapper>>();

        public static List<CoroutineWrapper> GetDelayedSetActives(this MonoBehaviour mb, GameObject target)
        {
            List<CoroutineWrapper> coroutines;
            if (delayedSetActives.TryGetValue(target, out coroutines))
            {
                return coroutines;
            }
            else
            {
                return new List<CoroutineWrapper>();
            }
        }

        public static void ClearDelayedSetActives(this MonoBehaviour mb, GameObject target)
        {
            var routines = GetDelayedSetActives(mb, target);
            routines.ForEach(r => r.owner.StopCoroutine(r.coroutine));
            routines.Clear();
        }

        public static Coroutine DelayedSetActive(this MonoBehaviour mb, GameObject target, float delay, bool value, bool stopPreviousRoutines)
        {
            if (mb.gameObject == target)
            {
                Debug.LogWarning("[MonoBehaviourExtensions] The routine is called on the same object than targeted! All routines that are ran by the object will stop, when the object is disabled!");
            }
            CoroutineWrapper newRoutine = null;
            if (mb.isActiveAndEnabled)
            {
                if (stopPreviousRoutines)
                {
                    ClearDelayedSetActives(mb, target);
                }
                if (!delayedSetActives.ContainsKey(target))
                {
                    // If no routines is found, add an empty list
                    delayedSetActives.Add(target, new List<CoroutineWrapper>());
                }
                newRoutine = new CoroutineWrapper(mb.DelayedMethod(() =>
                {
                    target.SetActive(value);
                    // Remove the routine from the list
                    delayedSetActives[target].Remove(newRoutine);
                }, delay), mb);
                // Add the routine to the list
                delayedSetActives[target].Add(newRoutine);
            }
            return newRoutine.coroutine;
        }

        public static Coroutine DelayedMethod(this MonoBehaviour mb, Action method, WaitForSecondsRealtime wait)
        {
            Coroutine coroutine = null;
            if (mb.isActiveAndEnabled)
            {
                coroutine = mb.StartCoroutine(DelayedCoroutine(method, wait));
            }
            return coroutine;
        }

        public static Coroutine DelayedMethod(this MonoBehaviour mb, Action method, WaitForSeconds wait)
        {
            Coroutine coroutine = null;
            if (mb.isActiveAndEnabled)
            {
                coroutine = mb.StartCoroutine(DelayedCoroutine(method, wait));
            }
            return coroutine;
        }

        public static Coroutine DelayedMethod(this MonoBehaviour mb, Action method, float delay, bool isTimeScaleIndependent = false)
        {
            Coroutine coroutine = null;
            if (mb.isActiveAndEnabled)
            {
                coroutine = mb.StartCoroutine(DelayedCoroutine(method, delay, isTimeScaleIndependent));
            }
            return coroutine;
        }

        public static Coroutine WaitingConditionalMethod(this MonoBehaviour mb, Action method, Func<bool> condition, float? timeout = null, Action callbackIfTimedOut = null, bool isTimeScaleIndependent = false)
        {
            Coroutine coroutine = null;
            if (mb.isActiveAndEnabled)
            {
                coroutine = mb.StartCoroutine(ConditionalCoroutine(method, condition, timeout, callbackIfTimedOut, isTimeScaleIndependent));
            }
            return coroutine;
        }

        private static IEnumerator DelayedCoroutine(Action method, WaitForSecondsRealtime wait)
        {
            yield return wait;
            method();
        }

        private static IEnumerator DelayedCoroutine(Action method, WaitForSeconds wait)
        {
            yield return wait;
            method();
        }

        private static IEnumerator DelayedCoroutine(Action method, float delay, bool isTimeScaleIndependent = false)
        {
            if (isTimeScaleIndependent)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }
            method();
        }

        private static IEnumerator ConditionalCoroutine(Action method, Func<bool> condition, float? timeout = null, Action callbackIfTimedOut = null, bool isTimeScaleIndependent = false)
        {
            if (timeout.HasValue)
            {
                float timer = 0;
                while (timer < timeout)
                {
                    if (condition())
                    {
                        method();
                        yield break;
                    }
                    timer += isTimeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return null;
                }
                if (callbackIfTimedOut != null) { callbackIfTimedOut(); }
                yield return null;
            }
            else
            {
                yield return new WaitWhile(condition);
                method();
            }
        }
    }
}

