using UnityEngine;
using UnityEngine.UI;

namespace ItchyOwl.Auxiliary
{
    [RequireComponent(typeof(Text))]
    public class SimpleFPSCounter : MonoBehaviour
    {
        public float updateDelay = 0.5f;

        private int fps;
        private int currentFPS;
        private float fpsNextPeriod;
        private Text text;

        private void Start()
        {
            fpsNextPeriod = Time.realtimeSinceStartup + updateDelay;
            text = GetComponent<Text>();
        }

        private void Update()
        {
            fps++;
            if (Time.realtimeSinceStartup > fpsNextPeriod)
            {
                CalculateFPS();
                UpdateText();
                ResetFPS();
            }
        }

        private void CalculateFPS()
        {
            currentFPS = Mathf.RoundToInt(fps / updateDelay);
        }

        private void ResetFPS()
        {
            fps = 0;
            fpsNextPeriod += updateDelay;
        }

        private void UpdateText()
        {
            text.text = string.Format("{0} FPS", currentFPS);
        }
    }
}
