using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using ItchyOwl.General;

namespace ItchyOwl.UI
{
    /// <summary>
    /// Manages selecting IToggleables created by a UIElementFactory, where only one can be simultaneously active.
    /// </summary>
    public class UISelector<T> : MonoBehaviour where T : Component, IToggleable
    {
        // Cannot use UISelector<T> here, because it's not serialized -> cannot access from inspector.
        public GameObject syncWithOtherSelector;
        private UISelector<T> _otherSelector;
        /// <summary>
        /// Note: In order to get your custom selector class via this property, you have to override the property in the inheritant class, like this:
        /// public new UICustomSelector OtherSelector { get { return base.OtherSelector as UICustomSelector; } }
        /// This is not necessary, if you don't use overridden methods.
        /// </summary>
        public virtual UISelector<T> OtherSelector
        {
            get
            {
                if (_otherSelector == null && syncWithOtherSelector != null)
                {
                    _otherSelector = syncWithOtherSelector.GetComponent<UISelector<T>>();
                }
                return _otherSelector;
            }
        }
        public T SelectedElement { get; protected set; }

        protected List<T> selectableElements = new List<T>();
        protected bool initReady;

        public event EventHandler<EventArg<T>> ElementSelected = (sender, args) => {  };
        public event EventHandler<EventArg<T>> ElementDeselected = (sender, args) => {  };

        private UIElementFactory<T> _elementFactory;
        /// <summary>
        /// Note: In order to get your custom factory class via this property, you have to override the property in the inheritant class, like this:
        /// public new UICustomFactory ElementFactory { get { return base.ElementFactory as UICustomFactory; } }
        /// This is not necessary, if you don't use overridden methods.
        /// </summary>
        public virtual UIElementFactory<T> ElementFactory
        {
            get
            {
                if (!initReady) { Init(); }
                return _elementFactory;
            }
        }

        protected virtual void Awake()
        {
            if (!initReady) { Init(); }
        }

        protected virtual void OnDestroy()
        {
            _elementFactory.ElementCreated -= OnElementCreated;
            _elementFactory.ElementDestroyed -= OnElementDestroyed;
            if (OtherSelector != null)
            {
                OtherSelector.ElementSelected -= OnOtherSelectorElementSelected;
            }
        }

        protected virtual void Init()
        {
            if (initReady) { return; }
            _elementFactory = GetComponentInChildren<UIElementFactory<T>>(includeInactive: true);
            _elementFactory.ElementCreated += OnElementCreated;
            _elementFactory.ElementDestroyed += OnElementDestroyed;
            if (OtherSelector != null)
            {
                OtherSelector.ElementSelected += OnOtherSelectorElementSelected;
            }
            initReady = true;
        }

        #region Selecting and deselecting
        public virtual void Deselect()
        {
            if (!initReady) { Init(); }
            if (SelectedElement != null)
            {
                Deselect(SelectedElement);
            }
        }

        protected virtual void Deselect(T element)
        {
            if (element == SelectedElement)
            {
                SelectedElement = null;
            }
            if (element.IsOn)
            {
                // Note that this is recursive: SetOff -> OnElementToggled -> Deselect
                element.SetOff();
                return;
            }
            ElementDeselected(this, new EventArg<T>(element));
        }

        public virtual void Select(T element)
        {
            if (!initReady) { Init(); }
            if (element == null) { return; }
            // Only one element can be simultaneously active.
            if (SelectedElement != null)
            {
                Deselect();
            }
            if (!element.IsOn)
            {
                // Note that this is recursive: SetOn -> OnElementToggled -> Select
                element.SetOn();
                return;
            }
            SelectedElement = element;
            ElementSelected(this, new EventArg<T>(element));
        }

        public virtual T SelectFirst()
        {
            var element = ElementFactory.Elements.FirstOrDefault();
            if (element != null)
            {
                Select(element);
            }
            return element;
        }
        #endregion

        #region Organizing
        public virtual void ReverseButtonOrder()
        {
            selectableElements.Reverse();
            UpdateTransformOrder();
        }

        public virtual void OrganizeButtons(bool enabledOnTop = true)
        {
            if (!initReady) { Init(); }
            if (enabledOnTop)
            {
                selectableElements = selectableElements.OrderByDescending(b => b.IsOn).ToList();
            }
            else
            {
                selectableElements = selectableElements.OrderBy(b => b.IsOn).ToList();
            }
            UpdateTransformOrder();
        }

        private void UpdateTransformOrder()
        {
            for (int i = 0; i < selectableElements.Count; i++)
            {
                selectableElements[i].transform.SetSiblingIndex(i);
            }
        }
        #endregion

        #region Registering and unregistering
        private void RegisterElement(T element)
        {
            if (selectableElements.Contains(element)) { return; }
            selectableElements.Add(element);
            element.Toggled += OnElementToggled;
            // Ensure that the element starts in the disabled state.
            if (element.IsOn)
            {
                element.SetOff();
            }
        }

        private void UnregisterElement(T element)
        {
            selectableElements.Remove(element);
            element.Toggled -= OnElementToggled;
        }
        #endregion

        #region Events
        private void OnElementCreated(object sender, EventArg<T> args)
        {
            RegisterElement(args.arg);
        }

        private void OnElementDestroyed(object sender, EventArg<T> args)
        {
            var element = args.arg;
            if (SelectedElement == element)
            {
                Deselect(element);
            }
            UnregisterElement(args.arg);
        }

        private void OnElementToggled(object sender, ToggleEvent args)
        {
            var element = sender as T;
            if (element == null) { return; }
            bool isOn = args.value;
            if (isOn)
            {
                Select(element);
            }
            else
            {
                Deselect(element);
            }
        }

        private void OnOtherSelectorElementSelected(object sender, EventArg<T> args)
        {
            Deselect();
        }
        #endregion
    }
}
