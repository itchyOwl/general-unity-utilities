using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Extended and modified version of the standard asset.
    /// TODO: use StringBuilder so that there's no unnecessary allocations.
    /// </summary>
    public class FPSCounter : Singleton<FPSCounter>
    {
        public bool storeRecord = true;
        public float recordStorageInSeconds = 60;
        public int minRecordSize = 5;
        public Text text;
        public bool showText = true;

        private int recordCounter;
        private float recordNextPeriod;

        private int fpsAccumulator;
        private float fpsNextPeriod;

        public static int CurrentFPS { get; private set; }
        public static int AverageFPS
        {
            get
            {
                if (totalRecord.Count == 0) { return CurrentFPS; }
                else if (totalRecord.Count < Instance.minRecordSize) { return PreviousAvgFPS; }
                else { return (int)totalRecord.Average(r => r.fps); }
            }
        }

        private static int _previousAvgFPS;
        private static int PreviousAvgFPS
        {
            get
            {
                if (_previousAvgFPS == 0) { return CurrentFPS; }
                else { return _previousAvgFPS; }
            }
        }

        private static string display = "{0} FPS";
        private static float fpsMeasurePeriod = 0.5f;
        private static List<Record> totalRecord = new List<Record>();

        // On iOS, we have to use reference types with IEnumerable.Average() method.
        private struct Record
        {
            public int fps;

            public Record(int fps)
            {
                this.fps = fps;
            }
        }

        protected override void Start()
        {
            base.Start();
            fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            recordNextPeriod = Time.realtimeSinceStartup + recordStorageInSeconds;
            if (text == null) { text = GetComponent<Text>(); }
        }

        protected override void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                totalRecord = new List<Record>();
                CurrentFPS = 0;
                _previousAvgFPS = 0;
            }
            base.OnDestroy();
        }

        private void Update()
        {
            text.enabled = showText;
            // measure average frames per second
            fpsAccumulator++;
            if (Time.realtimeSinceStartup > fpsNextPeriod)
            {
                CurrentFPS = (int) (fpsAccumulator / fpsMeasurePeriod);
                if (storeRecord)
                {
                    totalRecord.Add(new Record(CurrentFPS));
                }
                fpsAccumulator = 0;
                fpsNextPeriod += fpsMeasurePeriod;
                text.text = string.Format(display, CurrentFPS);
            }
            if (storeRecord)
            {
                recordCounter++;
                if (Time.realtimeSinceStartup > recordNextPeriod)
                {
                    _previousAvgFPS = AverageFPS;
                    totalRecord.Clear();
                    recordCounter = 0;
                    recordNextPeriod += recordStorageInSeconds;
                }
            }
        }
    }
}
