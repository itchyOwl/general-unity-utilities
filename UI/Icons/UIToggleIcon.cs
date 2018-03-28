using System;
using ItchyOwl.General;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ItchyOwl.UI
{
    /// <summary>
    /// Inherit or use this class for toggleable icons with on/off states.
    /// </summary>
    public class UIToggleIcon : UISelectableIcon, IToggleable, IPointerDownHandler, IPointerUpHandler
    {
        public SelectionMode selectionMode;
        public SelectionMode deselectionMode;

        public enum SelectionMode
        {
            None,
            OnPointerDown,
            OnPointerUp,
            OnSelection
        }

        public bool IsOn { get { return IsSelected; } }

        public event EventHandler<ToggleEvent> Toggled;

        // We have to explicitly implement the event
        event EventHandler<ToggleEvent> IToggleable.Toggled
        {
            add { Toggled += value; }
            remove { Toggled -= value; }
        }

        public void Toggle()
        {
            if (!IsOn)
            {
                SetOn();
            }
            else
            {
                SetOff();
            }
        }

        public virtual void SetOn()
        {
            Select();
        }

        public virtual void SetOff()
        {
            Deselect();
        }

        public override void Select()
        {
            bool wasSelected = IsSelected;
            base.Select();
            if (!wasSelected)
            {
                if (Toggled != null)
                {
                    Toggled(this, new ToggleEvent(true));
                }
            }
        }

        public override void Deselect()
        {
            bool wasSelected = IsSelected;
            base.Deselect();
            if (wasSelected)
            {
                if (Toggled != null)
                {
                    Toggled(this, new ToggleEvent(false));
                }
            }
        }

        public override void OnSelect(BaseEventData data)
        {
            if (selectionMode == SelectionMode.OnSelection)
            {
                Select();
            }
        }

        public override void OnDeselect(BaseEventData data)
        {
            if (deselectionMode == SelectionMode.OnSelection)
            {
                Deselect();
            }
        }

        public virtual void OnPointerDown(PointerEventData data)
        {
            CheckSelection(SelectionMode.OnPointerDown);
        }

        public virtual void OnPointerUp(PointerEventData data)
        {
            CheckSelection(SelectionMode.OnPointerUp);
        }

        protected void CheckSelection(SelectionMode mode)
        {
            if (IsOn)
            {
                if (deselectionMode == mode)
                {
                    Deselect();
                }
            }
            else if (selectionMode == mode)
            {
                Select();
            }
        }
    }
}
