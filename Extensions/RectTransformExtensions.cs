using UnityEngine;

namespace ItchyOwl.Extensions
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Centers the ui element relative to it's parent. Zeroes offsets, and sets pivot and anchors to 0.5.
        /// </summary>
        public static void Center(this RectTransform t)
        {
            t.PositionTo(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        }

        public static void PositionToTopLeft(this RectTransform t)
        {
            t.PositionTo(Vector2.up, Vector2.up, Vector2.up, Vector2.zero, Vector2.zero);
        }

        public static void PositionTo(this RectTransform t, Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var size = t.sizeDelta;
            t.offsetMin = offsetMin;
            t.offsetMax = offsetMax;
            t.pivot = pivot;
            t.anchorMin = anchorMin;
            t.anchorMax = anchorMax;
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

        public static void SetToMousePosition(this RectTransform t, Canvas canvas)
        {
            SetToScreenPosition(t, canvas, Input.mousePosition);
        }

        public static void SetToScreenPosition(this RectTransform t, Canvas canvas, Vector2 screenPos)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                t.position = screenPos;
            }
            else
            {
                Vector2 pos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out pos))
                {
                    t.position = canvas.transform.TransformPoint(pos);
                }
            }
        }

        /// <summary>
        /// Does a screenspace rect overlap a world space rect.
        /// Transforms the ui element into Rect and calls Overlaps(otherRect).
        /// If fourCornersArray is not provided, this function creates a new temporary array.
        /// </summary>
        public static bool OverlapsRect(this RectTransform t, Rect worldRect, Vector3[] fourCornersArray = null)
        {
            if (fourCornersArray == null)
            {
                fourCornersArray = new Vector3[4];
            }
            t.GetWorldCorners(fourCornersArray);
            var bottomLeft = fourCornersArray[0];
            var topRight = fourCornersArray[2];
            Vector2 size = new Vector2(topRight.x - bottomLeft.x, bottomLeft.y - topRight.y);
            Rect rect = new Rect(fourCornersArray[1], size);
            return worldRect.Overlaps(rect, allowInverse: true);
        }

        // Source: http://www.oguzkonya.com/2016/01/18/converting-unitys-recttransform-to-a-rectangle-in-screen-coordinates/
        /// <summary>
        /// Returns Rect, which is always in the screen space regardless of the canvas scalings.
        /// If fourCornersArray is not provided, this function creates a new temporary array.
        /// </summary>
        public static Rect GetScreenRect(this RectTransform t, Canvas canvas, Vector3[] fourCornersArray = null)
        {
            if (fourCornersArray == null)
            {
                fourCornersArray = new Vector3[4];
            }
            t.GetWorldCorners(fourCornersArray);
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            var screenCorners = new Vector3[2];
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(camera, fourCornersArray[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(camera, fourCornersArray[3]);
            screenCorners[0].y = Screen.height - screenCorners[0].y;
            screenCorners[1].y = Screen.height - screenCorners[1].y;
            return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
        }
    }
}

