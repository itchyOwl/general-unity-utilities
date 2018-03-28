using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ItchyOwl.UI
{
    public class UISelectableIcon : UIIcon, ISelectHandler, IDeselectHandler
    {
        public bool initOnAwake;
        /// <summary>
        /// If enabled, sets Button.colors all colors to Button disabled color, when the icon is deselected.
        /// </summary>
        public bool manipulateButtonColors = true;
        /// <summary>
        /// If enabled, sets Image.color to disabledImageColor, when the icon is deselected.
        /// </summary>
        public bool manipulateImageColor = false;
        public Color disabledImageColor = Color.grey;
        public GameObject highlight;

        protected ColorBlock normalColors;
        protected Color normalColor;

        public bool IsSelected { get; protected set; }

        public event EventHandler IconSelected = (sender, args) => { };
        public event EventHandler IconDeselected = (sender, args) => { };

        protected bool initReady;

        protected virtual void Awake()
        {
            if (initOnAwake)
            {
                Init();
            }
        }

        public virtual void Init()
        {
            if (initReady) { return; }
            normalColor = Image.color;
            normalColors = Button.colors;
            if (manipulateButtonColors)
            {
                Button.colors = SetColors(Button.colors.disabledColor);
            }
            if (manipulateImageColor)
            {
                Image.color = disabledImageColor;
            }
            if (highlight != null)
            {
                highlight.SetActive(false);
            }
            initReady = true;
        }

        public virtual void Select()
        {
            bool wasSelected = IsSelected;
            IsSelected = true;
            if (!wasSelected)
            {
                if (manipulateButtonColors)
                {
                    Button.colors = normalColors;
                }
                if (manipulateImageColor)
                {
                    Image.color = normalColor;
                }
                if (highlight != null)
                {
                    highlight.SetActive(true);
                }
                IconSelected(this, EventArgs.Empty);
            }
        }

        public virtual void Deselect()
        {
            bool wasSelected = IsSelected;
            IsSelected = false;
            if (manipulateButtonColors)
            {
                Button.colors = SetColors(Button.colors.disabledColor);
            }
            if (manipulateImageColor)
            {
                Image.color = disabledImageColor;
            }
            if (highlight != null)
            {
                highlight.SetActive(false);
            }
            if (wasSelected)
            {
                IconDeselected(this, EventArgs.Empty);
            }
        }

        public virtual void OnSelect(BaseEventData data)
        {
            Select();
        }

        public virtual void OnDeselect(BaseEventData data)
        {
            Deselect();
        }

        protected ColorBlock SetColors(Color color)
        {
            var colors = normalColors;
            colors.normalColor = color;
            colors.highlightedColor = color;
            colors.pressedColor = color;
            return colors;
        }
    }
}

