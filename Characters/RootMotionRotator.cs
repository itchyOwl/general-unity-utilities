using UnityEngine;
using System.Collections;
using System;
using ItchyOwl.Extensions;

namespace ItchyOwl.Characters
{
    /// <summary>
    /// NOTE: This is an old script, which might need refactoring.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class RootMotionRotator : MonoBehaviour
    {
        public Transform CurrentTarget { get; private set; }

        private Coroutine rotationRoutine;
        private Animator _animator;
        private Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
        }

        public void RotateToward(Transform target, Action callback = null)
        {
            if (rotationRoutine != null) { StopCoroutine(rotationRoutine); }
            rotationRoutine = StartCoroutine(RotateTowardRoutine(target, breakCondition: () => transform.IsFacing(target, angle: 2, lockYAxis: true), callback: callback));
        }

        protected void RotateTowardDir(Vector3 dir)
        {
            Vector3 cross = Vector3.Cross(transform.forward, dir.normalized);
            Animator.SetFloat("Turn", cross.y);
        }

        protected IEnumerator RotateTowardRoutine(Transform target = null, Func<bool> breakCondition = null, Action callback = null)
        {
            CurrentTarget = target;
            breakCondition = breakCondition ?? delegate () { return false; };
            while (!breakCondition())
            {
                var dir = CurrentTarget.position - transform.position;
                dir.y = transform.forward.y;
                RotateTowardDir(dir);
                Debug.DrawLine(transform.position, CurrentTarget.position, Color.black);
                yield return null;
            }
            if (callback != null) { callback(); }
            Animator.SetFloat("Turn", 0);
            CurrentTarget = null;
            rotationRoutine = null;
        }
    }
}
