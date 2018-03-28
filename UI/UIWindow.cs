using UnityEngine;
using System;

namespace ItchyOwl.UI
{
    /// <summary>
    /// Base class for all UIWindows
    /// </summary>
    public class UIWindow : MonoBehaviour
    {
        public bool registerOnStart;
        /// <summary>
        /// Set false, if the window is meant to persist between scenes.
        /// </summary>
        public bool unregisterOnReset = true;
        public bool startOpen;

        public bool IsRegistered { get; protected set; }
        public bool IsOpen { get; protected set; }
        public bool IsClosed { get { return !IsOpen; } }

        public static event EventHandler WindowOpen = (sender, args) => { };
        public static event EventHandler WindowClosed = (sender, args) => { };

        /// <summary>
        /// If overridden, be sure to call base.Start().
        /// </summary>
        protected virtual void Start()
        {
            if (!IsRegistered && registerOnStart)
            {
                Register();
            }
        }

        // Note that OnDestroy is launched only on MonoBehaviours that have once been active!
        protected virtual void OnDestroy()
        {
            Unregister();
        }

        /// <summary>
        /// Recursive registering.
        /// </summary>
        public virtual void Register()
        {
            if (!IsRegistered)
            {
                IsRegistered = true;
                GUIManager.RegisterWindow(this);
                if (startOpen)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// Recursive unregistering.
        /// </summary>
        public virtual void Unregister()
        {
            if (IsRegistered)
            {
                IsRegistered = false;
                GUIManager.UnregisterWindow(this);
            }
        }

        /// <summary>
        /// Call this only in the GUIManager.
        /// </summary>
        public virtual void Close()
        {
            gameObject.SetActive(false);
            if (IsOpen)
            {
                IsOpen = false;
                WindowClosed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Call this only in the GUIManager.
        /// </summary>
        public virtual void Open()
        {
            // Allow opening multiple times, because this window is not necessarily the last opened window.
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            if (IsClosed)
            {
                IsOpen = true;
                WindowOpen(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Call this only in the GUIManager.
        /// </summary>
        public virtual void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
}

