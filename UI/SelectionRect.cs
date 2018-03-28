using UnityEngine;

namespace ItchyOwl.UI
{
    public class SelectionRect : MonoBehaviour
    {
        public RectTransform visualTransform;
        public Camera worldCamera;

        void Start()
        {
            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }
            Clear();
        }

        public bool ContainsPoint(Vector3 point)
        {
            Vector2 screenPoint = worldCamera.WorldToScreenPoint(point);
            return RectTransformUtility.RectangleContainsScreenPoint(visualTransform, screenPoint);
        }

        public void Draw(Vector2 start, Vector2 end)
        {
            Vector2 size = new Vector2(end.x - start.x, end.y - start.y);
            visualTransform.anchoredPosition = start;
            visualTransform.sizeDelta = Vector2.one;
            visualTransform.localScale = new Vector3(size.x, size.y, 1);
        }

        public void Clear()
        {
            visualTransform.sizeDelta = Vector2.zero;
            visualTransform.localScale = Vector3.zero;
        }
    }
}
