using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ItchyOwl.Extensions;
using ItchyOwl.Auxiliary;

namespace ItchyOwl.UI
{
    public class NotificationFactory : MonoBehaviour
    {
        public GameObject prefab;
        public string parentName = "Notifications";
        public float spacing = 50;
        public int maxNumberOfNotifications = 5;

        private HashSet<Text> notifications = new HashSet<Text>();
        //private HashSet<Text> notifications = new HashSet<Text>();
        private List<Vector2> allPositions = new List<Vector2>();
        private Dictionary<RectTransform, Vector2> reservedPositions = new Dictionary<RectTransform, Vector2>();
        private bool positionsCalculated;

        public Text CreateNotification(string msg, Color? color = null, int size = 20, bool onlyOneInstanceAllowed = false, Canvas canvas = null, float fullAlphaTime = 5, float fadeTime = 0.5f, Tween.EasingMode easing = Tween.EasingMode.Smooth)
        {
            if (canvas == null)
            {
                canvas = GUIManager.DefaultCanvas;
            }
            if (onlyOneInstanceAllowed)
            {
                Text existingNote = notifications.Where(n => n.text.Equals(msg)).FirstOrDefault();
                if (existingNote != null)
                {
                    Debug.Log("NotificationFactory: An existing notification with the same string found.");
                    return existingNote;
                }
            }
            // Instantiate and setup transforms
            GameObject notificationParent = GameObject.Find(parentName);
            if (notificationParent == null)
            {
                Debug.Log(string.Format("NotificationFactory: Could not find a notification parent with the name {0}. Creating one...", parentName));
                notificationParent = new GameObject(parentName);
                var parentTransform = notificationParent.AddComponent<RectTransform>();
                parentTransform.SetParent(canvas.transform, worldPositionStays: false);
            }
            notificationParent.transform.SetAsLastSibling();
            var noteGO = Instantiate(prefab, notificationParent.transform);
            var rectT = noteGO.transform as RectTransform;
            rectT.SetAsLastSibling();
            if (!positionsCalculated) { CalculatePositions(rectT.anchoredPosition); }
            if (reservedPositions.Count == allPositions.Count)
            {
                Debug.LogWarning("NotificationFactory: Too many notifications! Please increase the maxNumberOfNotifications or spawn less notifications at a time.");
            }
            // Get the first free position
            var pos = allPositions.FirstOrDefault(p => !reservedPositions.ContainsValue(p));
            rectT.anchoredPosition = pos;
            reservedPositions.Add(rectT, pos);
            // Set colors and effects
            var note = noteGO.GetComponent<Text>();
            color = color ?? Color.white;
            note.color = color.Value;
            note.fontSize = size;
            // Set text
            note.text = msg;
            // Set fading and define the destructor as a callback
            var fader = noteGO.GetOrAddComponent<UIFader>();
            fader.fullAlphaTime = fullAlphaTime;
            fader.pingPong = true;
            fader.easing = easing;
            fader.fullAlphaTime = fullAlphaTime;
            fader.FadeTo(1, fadeTime, () =>
            {
                notifications.Remove(note);
                reservedPositions.Remove(rectT);
                Destroy(noteGO);
            });
            notifications.Add(note);
            return note;
        }

        private void CalculatePositions(Vector2 basePosition)
        {
            for (int i = 1; i <= maxNumberOfNotifications; i++)
            {
                int notificationsOnThisSide = Mathf.FloorToInt(i / 2);
                float spacing = notificationsOnThisSide * this.spacing;
                Vector2 newPos = i % 2 == 0 ?
                    new Vector2(basePosition.x, basePosition.y + spacing) :
                    new Vector2(basePosition.x, basePosition.y - spacing);
                allPositions.Add(newPos);
            }
            positionsCalculated = true;
        }
    }
}

