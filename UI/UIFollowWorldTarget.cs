using UnityEngine;
using ItchyOwl.Extensions;
using System;

namespace ItchyOwl.UI
{
    public class UIFollowWorldTarget : MonoBehaviour
    {
        public bool debug;
        public Transform target;
        /// <summary>
        /// If target is defined, this will be overridden.
        /// </summary>
        public Vector3 targetPos;
        public Camera worldCamera;
        public Vector3 offsetLocalWorld;
        public Vector2 offsetScreen;
        public bool restrictMovementOnScreen;
        public bool initOnStart;
        public UpdateMode updateMode;
        [Tooltip("Used for testing the line of view obstruction.")]
        public LayerMask obstructionLayers;

        public enum UpdateMode
        {
            Update,
            LateUpdate
        }

        public event Action InView;
        public event Action OutOfView;

        public Canvas Canvas { get; private set; }
        private RectTransform canvasRectT;
        private RectTransform rectT;
        private Vector2 elementHalfSize;
        private Vector2 canvasHalfSize;
        private bool initReady;

        public void Init(Canvas canvas = null)
        {
            rectT = GetComponent<RectTransform>();
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }
            canvasRectT = canvas.transform as RectTransform;
            Canvas = canvas;
            rectT.Center();
            elementHalfSize = rectT.sizeDelta / 2;
            canvasHalfSize = canvasRectT.sizeDelta / 2;
            initReady = true;
        }

        private void Start()
        {
            if (initOnStart)
            {
                Init();
            }
        }

        private void Update()
        {
            if (!initReady) { return; }
            if (updateMode != UpdateMode.Update) { return; }
            Follow();
        }

        private void LateUpdate()
        {
            if (!initReady) { return; }
            if (updateMode != UpdateMode.LateUpdate) { return; }
            Follow();
        }

        private void Follow()
        {
            if (worldCamera == null) { return; }
            if (target != null)
            {
                targetPos = target.position;
            }
            if (IsTargetInFieldOfView())
            {
                IsTargetVisible = !IsLineOfSightObstructed();
                if (IsTargetVisible)
                {
                    var screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, targetPos + offsetLocalWorld) / Canvas.scaleFactor;
                    var anchoredPosition = screenPoint - canvasHalfSize + offsetScreen;
                    if (restrictMovementOnScreen)
                    {
                        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, -canvasHalfSize.x + elementHalfSize.x, canvasHalfSize.x - elementHalfSize.x);
                        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, -canvasHalfSize.y + elementHalfSize.y, canvasHalfSize.y - elementHalfSize.y);
                    }
                    rectT.anchoredPosition = anchoredPosition;
                }
            }
            else
            {
                IsTargetVisible = false;
                // Set out of the view port
                rectT.anchoredPosition = new Vector2(-Screen.width, Screen.height);
            }
            if (debug)
            {
                if (IsTargetVisible)
                {
                    Debug.DrawLine(worldCamera.transform.position, targetPos, Color.green);
                }
                else
                {
                    Debug.DrawLine(worldCamera.transform.position, targetPos, Color.red);
                }
            }
        }

        private bool _isTargetVisible;
        public bool IsTargetVisible
        {
            get
            {
                return _isTargetVisible;
            }
            set
            {
                if (value != _isTargetVisible)
                {
                    if (value)
                    {
                        if (InView != null)
                        {
                            InView();
                        }
                    }
                    else
                    {
                        if (OutOfView != null)
                        {
                            OutOfView();
                        }
                    }
                }
                _isTargetVisible = value;
            }
        }

        private bool IsTargetInFieldOfView()
        {
            return Vector3.Dot(worldCamera.transform.forward, Vector3.Normalize(targetPos - worldCamera.transform.position)) > 0;
        }

        private bool IsLineOfSightObstructed()
        {
            return Physics.Linecast(worldCamera.transform.position, targetPos, obstructionLayers);
        }
    }
}
