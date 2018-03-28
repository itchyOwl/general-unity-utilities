using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.UI
{
    public class UISelectionRect : MonoBehaviour
    {
        // Fetch for example an empty UI Image component that has been given a color.
        public RectTransform visualTransform;
        public Camera worldCamera;

        void Awake()
        {
            Clear();
        }

        /// <summary>
        /// Is point in the world space overlapping the UI rect in screen space?
        /// </summary>
        public bool ContainsPoint(Vector3 point)
        {
            Vector2 screenPoint = worldCamera.WorldToScreenPoint(point);
            return RectTransformUtility.RectangleContainsScreenPoint(visualTransform, screenPoint);
        }

        /// <summary>
        /// Does a screenspace rect overlap the selection rect?
        /// Uses RectTransform.OverlapsRect() extension method.
        /// </summary>
        public bool OverlapsRect(Rect rect)
        {
            return visualTransform.OverlapsRect(rect);
        }

        /// <summary>
        /// Draws an UI rect between these two points.
        /// </summary>
        public void Draw(Vector2 start, Vector2 end)
        {
            Vector2 size = new Vector2(end.x - start.x, end.y - start.y);
            visualTransform.anchoredPosition = start;
            visualTransform.sizeDelta = Vector2.one;
            visualTransform.localScale = size;
        }

        /// <summary>
        /// Clears the rect.
        /// </summary>
        public void Clear()
        {
            visualTransform.sizeDelta = Vector2.zero;
            visualTransform.localScale = Vector2.zero;
        }
    }
}

