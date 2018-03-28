using UnityEngine;
using System;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    public class AnimationLayerTweener : Tweener
    {
        [SerializeField]
        private Animator _animator;
        public Animator Animator
        {
            get
            {
                _animator = gameObject.GetReferenceTo(_animator, seekChildren: true, includeInactive: true);
                return _animator;
            }
            set
            {
                _animator = value;
            }
        }

        /// <summary>
        /// Note that if the startWeight is not defined and if this is the first time the tweener starts, last value will be 0! You may want to define the startWeight, if you are tweening to 0.
        /// </summary>
        public void TweenLayerTo(string layerName, float targetWeight, float? startWeight = null, float duration = 1, Tween.EasingMode easing = Tween.EasingMode.Smooth, Action readyCallback = null, Action abortCallback = null)
        {
            TweenLayerTo(Animator.GetLayerIndex(layerName), targetWeight, startWeight, duration, easing, readyCallback, abortCallback);
        }

        /// <summary>
        /// Note that if the startWeight is not defined and if this is the first time the tweener starts, last value will be 0! You may want to define the startWeight, if you are tweening to 0.
        /// </summary>
        public void TweenLayerTo(int layerIndex, float targetWeight, float? startWeight = null, float duration = 1, Tween.EasingMode easing = Tween.EasingMode.Smooth, Action readyCallback = null, Action abortCallback = null)
        {
            TweenTo(
                tweenId: layerIndex,
                to: targetWeight,
                from: startWeight,
                duration: duration,
                easing: easing,
                updateCallback: value => Animator.SetLayerWeight(layerIndex, value),
                readyCallback: readyCallback,
                abortCallback: abortCallback);
        }
    }
}
