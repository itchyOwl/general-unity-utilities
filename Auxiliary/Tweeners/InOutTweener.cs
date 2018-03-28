using System;

namespace ItchyOwl.Auxiliary
{
    public class InOutTweener : Tweener
    {
        public float targetWeight = 1;
        public Tween.EasingMode easingIn = Tween.EasingMode.Linear;
        public Tween.EasingMode easingOut = Tween.EasingMode.Linear;
        public float durationIn = 1;
        public float durationOut = 1;

        public void TweenInOut(Action<float> updateMethod, Action resetAction = null)
        {
            TweenTo(tweenId: 0, to: targetWeight, duration: durationIn, easing: easingIn, updateCallback: updateMethod, abortCallback: resetAction, readyCallback: () =>
                TweenTo(tweenId: 0, from: targetWeight, to: 0, duration: durationOut, easing: easingOut, updateCallback: updateMethod, abortCallback: resetAction, readyCallback: resetAction));
        }
    }
}
