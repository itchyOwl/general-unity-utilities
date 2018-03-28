using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using ItchyOwl.Extensions;
using ItchyOwl.General;
using UnityEngine.UI;

namespace ItchyOwl.UI
{
    public class UIDraggableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public float sizeMultiplier = 1.5f;
        public HashSet<Transform> allowedSlots = new HashSet<Transform>();
        public bool disablePointerClickFunctionality;
        public bool allowOnlyLeftClickDragging = true;
        /// <summary>
        /// Enable when the slot is a child of a scroll view or in similar cases where the player uses the parent as the visual cue for placing the element. 
        /// </summary>
        public bool useSlotParentForOverlapChecks;

        protected RectTransform _rectT;
        public RectTransform RectT
        {
            get
            {
                if (_rectT == null)
                {
                    _rectT = transform as RectTransform;
                }
                return _rectT;
            }
        }
        public Transform OriginalParent { get; protected set; }
        protected Vector2 originalSize;
        protected Transform _currentSlot;
        public Transform PreviousSlot { get; private set; }
        public Transform CurrentSlot
        {
            get
            {
                return _currentSlot;
            }
            protected set
            {
                PreviousSlot = _currentSlot;
                _currentSlot = value;
            }
        }
        public bool IsBeingDragged { get; protected set; }

        public event EventHandler<EventArg<Transform>> PlacedIntoSlot = (sender, args) => { };
        public event EventHandler<EventArg<Transform>> RemovedFromSlot = (sender, args) => { };
        public event EventHandler<EventArg<Transform>> RestoredTo = (sender, args) => { };

        protected virtual void Start()
        {
            originalSize = RectT.sizeDelta;
            if (OriginalParent == null)
            {
                SetOriginalParent(transform.parent);
            }
            CurrentSlot = transform.parent;
        }

        #region Public methods
        public virtual void SetOriginalParent(Transform t)
        {
            OriginalParent = t;
        }

        public virtual void PlaceIntoSlot(Transform slot)
        {
            Reparent(slot);
            CurrentSlot = slot;
            PlacedIntoSlot(this, new EventArg<Transform>(CurrentSlot));
        }

        /// <summary>
        /// Restores the icon to the original parent.
        /// </summary>
        public virtual void Restore(Transform parent = null)
        {
            if (parent != null)
            {
                OriginalParent = parent;
            }
            Reparent(OriginalParent);
            CurrentSlot = OriginalParent;
            RestoredTo(this, new EventArg<Transform>(OriginalParent));
        }
        #endregion

        #region Interface implementations, UI events
        public void OnBeginDrag(PointerEventData data)
        {
            if (allowOnlyLeftClickDragging && data.button != PointerEventData.InputButton.Left) { return; }
            IsBeingDragged = true;
            RemoveFromSlot();
            // This hack prevents the grid layout from messing up with the position when the item is reparented.
            var gridLayout = RectT.parent.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                gridLayout.enabled = false;
            }
            RectT.SetParent(GUIManager.DefaultCanvas.transform, worldPositionStays: true);
            if (gridLayout != null)
            {
                gridLayout.enabled = true;
            }
            RectT.SetToScreenPosition(GUIManager.DefaultCanvas, data.position);
            RectT.sizeDelta = originalSize * sizeMultiplier;
        }

        public void OnDrag(PointerEventData data)
        {
            if (IsBeingDragged)
            {
                RectT.anchoredPosition += data.delta;
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            IsBeingDragged = false;
            // Reset the scale, because there seems to be a bug that alters the scale when the element is reparented.
            RectT.localScale = Vector3.one;
            RectT.sizeDelta = originalSize;
            var slot = GetNearestAllowedSlot();
            if (slot != null)
            {
                var slotT = useSlotParentForOverlapChecks ? slot.parent as RectTransform : slot.transform as RectTransform;
                var slotRect = slotT.GetScreenRect(GUIManager.DefaultCanvas);
                var iconRect = RectT.GetScreenRect(GUIManager.DefaultCanvas);
                if (slotRect.Overlaps(iconRect) || RectTransformUtility.RectangleContainsScreenPoint(slotT, Input.mousePosition))
                {
                    PlaceIntoSlot(slot);
                }
                else
                {
                    Restore();
                }
            }
            else
            {
                Restore();
            }
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (disablePointerClickFunctionality) { return; }
            if (data.button == PointerEventData.InputButton.Right)
            {
                var otherSlot = GetNearestAllowedSlot(skip: CurrentSlot);
                RemoveFromSlot();
                PlaceIntoSlot(otherSlot);
            }
        }
        #endregion

        protected virtual Transform GetNearestAllowedSlot(Transform skip = null)
        {
            return allowedSlots.Where(s => s != skip).OrderBy(s => s.GetScreenSpaceDistanceTo(transform)).FirstOrDefault();
        }

        protected virtual void RemoveFromSlot()
        {
            CurrentSlot = null;
            RemovedFromSlot(this, new EventArg<Transform>(PreviousSlot));
        }

        protected void Reparent(Transform parent)
        {
            RectT.SetParent(parent, worldPositionStays: false);
            RectT.Center();
        }
    }
}

