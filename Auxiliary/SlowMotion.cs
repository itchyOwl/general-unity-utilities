using System;
using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    // TODO: Slow motion conflicts with pausing.
    public class SlowMotion : Singleton<SlowMotion>
    {
        public KeyCode inputKey = KeyCode.None;
        public float slowMotionSpeed = 0.5f;

        private const float NORMAL_TIME_SCALE = 1;

        public static event Action SlowMotionActivated;
        public static event Action SlowMotionDeactivated;

        public static bool IsActive { get; private set; }

        public static void Activate(float speed = 0, float autoDeactivateAfter = 0)
        {
            if (IsActive) { return; }
            if (speed > 0)
            {
                Instance.slowMotionSpeed = speed;
            }
            // If the time scale is less than the slow motion speed, the speed would increase.
            // The game is probably paused.
            if (Time.timeScale < Instance.slowMotionSpeed) { return; }
            IsActive = true;
            Time.timeScale = Instance.slowMotionSpeed;
            if (SlowMotionActivated != null)
            {
                SlowMotionActivated();
            }
            if (autoDeactivateAfter > 0)
            {
                Instance.DelayedMethod(Deactivate, autoDeactivateAfter, isTimeScaleIndependent: true);
            }
        }

        public static void Deactivate()
        {
            if (!IsActive) { return; }
            IsActive = false;
            Time.timeScale = NORMAL_TIME_SCALE;
            if (SlowMotionDeactivated != null)
            {
                SlowMotionDeactivated();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(inputKey))
            {
                Activate();
            }
            else if (Input.GetKeyUp(inputKey))
            {
                Deactivate();
            }
        }
    }
}
