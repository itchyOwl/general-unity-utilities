using System;
using UnityEngine;
using UnityEngine.UI;
using ItchyOwl.Extensions;
using ItchyOwl.Auxiliary;

namespace ItchyOwl.UI
{
    public class UIFader : Tweener
    {
        [SerializeField]
        private Graphic _targetGraphic;
        public Graphic TargetGraphic
        {
            get
            {
                _targetGraphic = gameObject.GetReferenceTo(_targetGraphic, seekChildren: true);
                return _targetGraphic;
            }
            set
            {
                _targetGraphic = value;
            }
        }
        [SerializeField]
        private CanvasGroup _targetCanvasGroup;
        public CanvasGroup TargetCanvasGroup
        {
            get
            {
                _targetCanvasGroup = gameObject.GetReferenceTo(_targetCanvasGroup, seekChildren: true);
                return _targetCanvasGroup;
            }
            set
            {
                _targetCanvasGroup = value;
            }
        }
        public float startAlpha;
        public float fullAlphaTime;
        public Tween.EasingMode easing = Tween.EasingMode.Linear;
        public bool pingPong;
        public int pingPongCount = 1;

        private void Start()
        {
            SetNewAlpha(startAlpha);
        }

        public void FadeTo(float targetAlpha, float fadeTime, Action readyCallback = null, Action abortCallback = null)
        {
            TweenTo(
                tweenId: 0, 
                to: targetAlpha, 
                duration: fadeTime, 
                pingPong: pingPong, 
                waitBetweenTime: fullAlphaTime, 
                pingPongCount: pingPongCount, 
                easing: easing, 
                updateCallback: value => SetNewAlpha(value), 
                readyCallback: readyCallback, 
                abortCallback: abortCallback);
        }

        private void SetNewAlpha(float newAlpha)
        {
            var canvasGroup = TargetCanvasGroup;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = newAlpha;
            }
            else
            {
                TargetGraphic.color = new Color(TargetGraphic.color.r, TargetGraphic.color.g, TargetGraphic.color.b, newAlpha);
            }
        }
    }
}
