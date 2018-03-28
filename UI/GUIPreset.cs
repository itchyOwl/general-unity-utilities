using UnityEngine;

namespace ItchyOwl.UI
{
    public class GUIPreset : ScriptableObject
    {
        public RenderMode canvasRenderMode = RenderMode.ScreenSpaceOverlay;
        public bool pixelPerfect = true;
        public int maxNumberOfNotifications = 20;

        public GameObject notificationPrefab;
        public GameObject floatingTextPrefab;
    }
}
