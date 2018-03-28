using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections.Generic;
using ItchyOwl.Extensions;
using ItchyOwl.Auxiliary;
using ItchyOwl.SceneManagement;

namespace ItchyOwl.UI
{
    public class GUIManager : Singleton<GUIManager>
    {
        [SerializeField]
        protected GUIPreset preset;

        /// <summary>
        /// Use only this to access the preset, because it may be overrided.
        /// </summary>
        protected virtual GUIPreset GetPreset() { return preset; }

        private Canvas defaultCanvas;
        public static Canvas DefaultCanvas
        {
            get
            {
                if (Instance.defaultCanvas == null)
                {
                    Instance.defaultCanvas = FindObjectOfType<Canvas>();
                    if (Instance.defaultCanvas == null)
                    {
                        Debug.Log("[GUIManager] Could not find a canvas. Creating one...");
                        var canvasObject = new GameObject("DefaultCanvas");
                        canvasObject.transform.SetParent(Instance.transform);
                        Instance.defaultCanvas = canvasObject.AddComponent<Canvas>();
                        canvasObject.AddComponent<CanvasScaler>();
                        canvasObject.AddComponent<GraphicRaycaster>();
                        Instance.defaultCanvas.renderMode = Instance.GetPreset().canvasRenderMode;
                        Instance.defaultCanvas.pixelPerfect = Instance.GetPreset().pixelPerfect;
                    }
                }
                return Instance.defaultCanvas;
            }
        }

        private EventSystem eventSystem;
        public static EventSystem EventSystem
        {
            get
            {
                if (Instance.eventSystem == null)
                {
                    Instance.eventSystem = FindObjectOfType<EventSystem>();
                    if (Instance.eventSystem == null)
                    {
                        Debug.Log("[GUIManager] Could not find an event system. Creating one...");
                        var eventSystemObject = new GameObject("EventSystem");
                        eventSystemObject.transform.SetParent(Instance.transform);
                        Instance.eventSystem = eventSystemObject.AddComponent<EventSystem>();
                        eventSystemObject.AddComponent<StandaloneInputModule>();
                    }
                }
                return Instance.eventSystem;
            }
        }

        private NotificationFactory notificationFactory;
        protected NotificationFactory NotificationFactory
        {
            get
            {
                if (notificationFactory == null)
                {
                    notificationFactory = notificationFactory.GetSingleton("NotificationFactory");
                    if (GetPreset().notificationPrefab == null)
                    {
                        throw new Exception("[GUIManager] Notification prefab not defined!");
                    }
                    notificationFactory.prefab = GetPreset().notificationPrefab;
                    notificationFactory.maxNumberOfNotifications = GetPreset().maxNumberOfNotifications;
                }
                return notificationFactory;
            }
        }

        private FloatingTextFactory floatingTextFactory;
        protected FloatingTextFactory FloatingTextFactory
        {
            get
            {
                if (floatingTextFactory == null)
                {
                    floatingTextFactory = floatingTextFactory.GetSingleton("FloatingTextFactory");
                    if (GetPreset().floatingTextPrefab == null)
                    {
                        throw new Exception("[GUIManager] Floating text prefab not defined!");
                    }
                    floatingTextFactory.floatingTextPrefab = GetPreset().floatingTextPrefab;
                }
                return floatingTextFactory;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManagerExtensions.BeforeSceneUnload += OnBeforeSceneUnload;
            UIWindow.WindowClosed += OnWindowClosed;
        }

        protected override void Start()
        {
            base.Start();
            // Don't initialize in Awake, because it causes the singleton pattern to loop twice, because we access the singleton in the initialization.
            Init();
        }

        protected bool initReady;
        public virtual void Init()
        {
            if (initReady) { return; }
            // Ensure that there is an event system
            eventSystem = EventSystem;
            initReady = true;
            Debug.Log("[GUIManager] Initialisation ready");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManagerExtensions.BeforeSceneUnload -= OnBeforeSceneUnload;
            UIWindow.WindowClosed -= OnWindowClosed;
        }

        public void OnBeforeSceneUnload(Scene scene)
        {
            Debug.Log("[GUIManager] Scene unloaded. Unregistering windows.");
            Instance.allWindows.Where(w => w.unregisterOnReset).ForEachMod(w => UnregisterWindow(w));
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("[GUIManager] Scene loaded, initializing.");
            Init();
        }

        #region Windows
        private HashSet<UIWindow> allWindows = new HashSet<UIWindow>();
        private HashSet<UIWindow> recentlyClosedWindows = new HashSet<UIWindow>();
        private HashSet<UIWindow> previouslyOpenWindows = new HashSet<UIWindow>();

        public static IEnumerable<UIWindow> AllWindows { get { return Instance.allWindows; } }
        /// <summary>
        /// Windows that were closed when the last windows were opened.
        /// </summary>
        public static IEnumerable<UIWindow> RecentlyClosedWindows { get { return Instance.recentlyClosedWindows; } }
        /// <summary>
        /// Windows that were open before the last windows were opened.
        /// </summary>
        public static IEnumerable<UIWindow> PreviouslyOpenWindows { get { return Instance.previouslyOpenWindows; } }
        public static IEnumerable<UIWindow> CurrentlyOpenWindows { get { return Instance.allWindows.Where(w => w.IsOpen); } }
        public static IEnumerable<UIWindow> CurrentlyClosedWindows { get { return Instance.allWindows.Where(w => w.IsClosed); } }


        /// <summary>
        /// Window recursive registering.
        /// </summary>
        public static void RegisterWindow(UIWindow window)
        {
            if (window == null) { return; }
            if (isQuitting) { return; }
            if (!Instance.allWindows.Contains(window))
            {
                Instance.allWindows.Add(window);
                window.Register();
            }
        }

        /// <summary>
        /// Window recursive unregistering.
        /// </summary>
        public static void UnregisterWindow(UIWindow window)
        {
            if (window == null) { return; }
            if (isQuitting) { return; }
            if (Instance.allWindows.Contains(window))
            {
                Instance.allWindows.Remove(window);
                Instance.previouslyOpenWindows.Remove(window);
                Instance.recentlyClosedWindows.Remove(window);
                window.Unregister();
            }
        }

        public static void OpenPreviouslyOpenWindows()
        {
            OpenWindows(Instance.previouslyOpenWindows);
        }

        public static void OpenOnlyPreviouslyOpenWindows()
        {
            OpenOnlyWindows(Instance.previouslyOpenWindows);
        }

        public static void OpenRecentlyClosedWindows()
        {
            OpenWindows(Instance.recentlyClosedWindows);
        }

        public static void OpenOnlyRecentlyClosedWindows()
        {
            OpenOnlyWindows(Instance.recentlyClosedWindows);
        }

        public static void CloseAllWindows()
        {
            Instance.allWindows.ForEach(w => CloseWindow(w));
        }

        public static void OpenOnlyWindows(IEnumerable<UIWindow> windows)
        {
            CloseAllExcept(windows);
            OpenWindows(windows);
        }

        public static void OpenOnlyWindow(UIWindow window)
        {
            CloseAllExcept(window);
            window.Open();
        }

        public static void CloseAllExcept(UIWindow window)
        {
            CloseWindows(Instance.allWindows.Where(w => w != window));
        }

        public static void CloseAllExcept(IEnumerable<UIWindow> windows)
        {
            CloseWindows(Instance.allWindows.Where(w => !windows.Contains(w)));
        }

        public static void OpenWindows(IEnumerable<UIWindow> windows)
        {
            windows.ForEach(w => OpenWindow(w));
        }

        public static void OpenWindow(UIWindow window)
        {
            window.Open();
        }

        public static void CloseWindows(IEnumerable<UIWindow> windows)
        {
            var openWindows = windows.Where(w => w.IsOpen);
            if (openWindows.Any())
            {
                Instance.previouslyOpenWindows = CurrentlyOpenWindows.ToHashSet();
                Instance.recentlyClosedWindows = openWindows.ToHashSet();
                openWindows.ForEach(w => w.Close());
            }
        }

        public static void CloseWindow(UIWindow window)
        {
            if (window.IsOpen)
            {
                Instance.recentlyClosedWindows.Clear();
                Instance.recentlyClosedWindows.Add(window);
                Instance.previouslyOpenWindows.Add(window);
                window.Close();
            }
        }

        public static void ToggleWindow(UIWindow window)
        {
            if (window.IsOpen)
            {
                CloseWindow(window);
            }
            else
            {
                OpenWindow(window);
            }
        }

        private void OnWindowClosed(object sender, EventArgs args)
        {
            var window = sender as UIWindow;
            if (!Instance.recentlyClosedWindows.Contains(window))
            {
                // When the window closed via this manager, it should be in the collection.
                // If the window is not in the collection, it means that this is another command.
                Instance.recentlyClosedWindows.Clear();
                Instance.recentlyClosedWindows.Add(window);
            }
            if (!Instance.previouslyOpenWindows.Contains(window))
            {
                // When the window closed via this manager, it should be in the collection.
                Instance.previouslyOpenWindows.Add(window);
            }
        }
        #endregion

        #region Notifications
        public static Text CreateNotification(string msg, Color? color = null, int size = 30, bool onlyOneInstance = false, Canvas canvas = null, float fullAlphaTime = 5, float fadeTime = 0.5f, Tween.EasingMode easing = Tween.EasingMode.Smooth)
        {
            return Instance.NotificationFactory.CreateNotification(msg, color, size, onlyOneInstance, canvas, fullAlphaTime, fadeTime, easing);
        }

        public static Text CreateFloatingText(Vector3 worldPosition, string text, Color? color = null, int size = 20, Vector3? translation = null, Canvas canvas = null, Camera camera = null, float fadeTime = 0.5f, float fullAlphaTime = 1.5f, Tween.EasingMode fadeEasing = Tween.EasingMode.Smooth, Tween.EasingMode translationEasing = Tween.EasingMode.Linear, Action callback = null)
        {
            return Instance.FloatingTextFactory.CreateFloatingText(worldPosition, text, color, size, translation, canvas, camera, fadeTime, fullAlphaTime, fadeEasing, translationEasing, callback);
        }

        public static Text CreateFloatingText(Vector2 screenPosition, string text, Color? color = null, int size = 20, Vector2? translation = null, Canvas canvas = null, float fadeTime = 0.5f, float fullAlphaTime = 1.5f, Tween.EasingMode fadeEasing = Tween.EasingMode.Smooth, Tween.EasingMode translationEasing = Tween.EasingMode.Linear, Action callback = null)
        {
            return Instance.FloatingTextFactory.CreateFloatingText(screenPosition, text, color, size, translation, canvas, fadeTime, fullAlphaTime, fadeEasing, translationEasing, callback);
        }
        #endregion
    }
}
