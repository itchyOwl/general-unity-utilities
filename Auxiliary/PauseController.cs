using UnityEngine;
using System;

namespace ItchyOwl.Auxiliary
{
    public class PauseController : Singleton<PauseController>
    {
        public KeyCode pauseKey;
        public bool freezeTimeWhenPaused;

        /// <summary>
        /// Usually it's 1, but it can be something other, if we are manipulating it elsewhere.
        /// </summary>
        private static float normalTimeScale = 1;

        public static event EventHandler GamePaused = (sender, args) => Debug.Log("[PauseController] Pause");
        public static event EventHandler GameContinue = (sender, args) => Debug.Log("[PauseController] Continue");

        public static bool IsPaused { get; private set; }
        public static bool IsTimeFreezed { get; private set; }


        protected override void OnDestroy()
        {
            if (_instance == this)
            {
                GamePaused = (sender, args) => Debug.Log("[PauseController] Pause");
                GameContinue = (sender, args) => Debug.Log("[PauseController] Continue");
                Continue();
                FreezeTime(false);
            }
            base.OnDestroy();
        }

        private void Update()
        {
            if (Input.GetKeyDown(pauseKey))
            {
                TogglePause();
            }
        }

        public static void TogglePause()
        {
            if (IsPaused)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }

        public static void Pause()
        {
            if (IsPaused) { return; }
            IsPaused = true;
            GamePaused(Instance, EventArgs.Empty);
            if (Instance.freezeTimeWhenPaused)
            {
                FreezeTime(true);
            }
        }

        public static void Continue()
        {
            if (!IsPaused) { return; }
            IsPaused = false;
            GameContinue(Instance, EventArgs.Empty);
            FreezeTime(false);
        }

        public static void FreezeTime(bool enabled)
        {
            if (enabled)
            {
                if (!IsTimeFreezed)
                {
                    normalTimeScale = Time.timeScale;
                }
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = normalTimeScale;
            }
            IsTimeFreezed = enabled;
        }
    }
}

