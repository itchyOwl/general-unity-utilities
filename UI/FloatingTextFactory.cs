using UnityEngine;
using UnityEngine.UI;
using ItchyOwl.Extensions;
using ItchyOwl.Auxiliary;
using System;

namespace ItchyOwl.UI
{
    public class FloatingTextFactory : MonoBehaviour
    {
        public GameObject floatingTextPrefab;
        public string parentName = "FloatingTexts";
        public UIFollowWorldTarget.UpdateMode followerUpdateMode = UIFollowWorldTarget.UpdateMode.Update;

        public Text CreateFloatingText(Vector2 screenPosition, string text, Color? color = null, int size = 20, Vector2? translation = null, Canvas canvas = null, float fadeTime = 0.5f, float fullAlphaTime = 1.5f, Tween.EasingMode fadeEasing = Tween.EasingMode.Smooth, Tween.EasingMode translationEasing = Tween.EasingMode.Linear, Action callback = null)
        {
            return SetupFloatingText(false, screenPosition, text, color, size, translation, canvas, null, fadeTime, fullAlphaTime, fadeEasing, translationEasing, callback);
        }

        public Text CreateFloatingText(Vector3 worldPosition, string text, Color? color = null, int size = 20, Vector3? translation = null, Canvas canvas = null, Camera camera = null, float fadeTime = 0.5f, float fullAlphaTime = 1.5f, Tween.EasingMode fadeEasing = Tween.EasingMode.Smooth, Tween.EasingMode translationEasing = Tween.EasingMode.Linear, Action callback = null)
        {
            return SetupFloatingText(true, worldPosition, text, color, size, translation, canvas, camera, fadeTime, fullAlphaTime, fadeEasing, translationEasing, callback);
        }

        private Text SetupFloatingText(bool worldSpace, Vector3 position, string text, Color? color, int size, Vector3? translation, Canvas canvas, Camera camera, float fadeTime, float fullAlphaTime, Tween.EasingMode fadeEasing, Tween.EasingMode translationEasing, Action callback)
        {
            if (canvas == null)
            {
                canvas = GUIManager.DefaultCanvas;
            }
            // Parent
            GameObject parent = GameObject.Find(parentName);
            if (parent == null)
            {
                Debug.LogFormat("[FloatingTextFactory] Could not find a floating text parent with the name {0}. Creating one...", parentName);
                parent = new GameObject(parentName);
                parent.AddComponent<RectTransform>();
                parent.transform.SetParent(canvas.transform, worldPositionStays: false);
            }
            parent.transform.SetAsLastSibling();
            // Instance
            var go = Instantiate(floatingTextPrefab, parent.transform);
            var rectT = go.transform as RectTransform;
            rectT.SetAsLastSibling();
            rectT.Center();
            if (!worldSpace)
            {
                rectT.position = position;
            }
            // Text
            var textComponent = go.GetOrAddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = size;
            textComponent.color = color.HasValue ? color.Value : Color.white;
            // Follower
            var follower = worldSpace ? go.GetOrAddComponent<UIFollowWorldTarget>() : go.GetComponent<UIFollowWorldTarget>();
            if (worldSpace)
            {
                follower.enabled = true;
                follower.targetPos = position;
                follower.worldCamera = camera == null ? Camera.main : camera;
                follower.updateMode = followerUpdateMode;
                follower.Init(canvas);
            }
            else if (follower != null)
            {
                follower.enabled = false;
            }
            // Fading
            var fader = go.GetOrAddComponent<UIFader>();
            fader.fullAlphaTime = fullAlphaTime;
            fader.pingPong = true;
            fader.easing = fadeEasing;
            Action cb = () => Destroy(go);
            fader.FadeTo(1, fadeTime, cb, cb);
            // Translation
            if (translation.HasValue)
            {
                if (callback == null) { callback = () => { }; }
                var positionTweener = go.GetOrAddComponent<LinearPositionTweener>();
                positionTweener.easing = translationEasing;
                float time = fadeTime * 2 + fullAlphaTime;
                if (follower != null && follower.enabled)
                {
                    positionTweener.TweenPosition(position, translation.Value, translation.Value.magnitude, time, value => follower.targetPos = positionTweener.CurrentPoint, callbackWhenReady: callback);
                }
                else
                {
                    positionTweener.TweenPosition(position, translation.Value, translation.Value.magnitude, time, value => rectT.position = positionTweener.CurrentPoint, callbackWhenReady: callback);
                }
            }
            return textComponent;
        }
    }
}

