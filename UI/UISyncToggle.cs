using UnityEngine;
using UnityEngine.UI;
using System;
using ItchyOwl.General;

namespace ItchyOwl.UI
{
    /// <summary>
    /// Inherit this class to synchronize a Unity toggle element to the state of an IToggleable.
    /// Be sure to call base.Awake() if the inherited class implements that unity event.
    /// </summary>
    public abstract class UISyncToggle : MonoBehaviour
    {
        /// <summary>
        /// The target should have a script that implements IToggleable attached to it.
        /// </summary>
        public GameObject target;
        /// <summary>
        /// Unity toggle to sync with.
        /// </summary>
        public Toggle toggle;

        /// <summary>
        /// This action is executed when the toggle state is changed.
        /// The current toggle state is provided as an argument.
        /// Note that this callback is called when the target state is changed. selfToggleAction is called when the toggle is clicked.
        /// Example: toggleAction = state => targetGameObject.SetActive(state);
        /// </summary>
        protected Action<bool> targetToggleAction;

        /// <summary>
        /// This action is called when the toggle is clicked [toggle.onValueChanged.AddListener(state => selfToggleAction(state));]
        /// Example: toggleAction = state => targetGameObject.SetActive(state);
        /// </summary>
        protected Action<bool> selfToggleAction;

        protected virtual void Awake()
        {
            selfToggleAction = selfToggleAction ?? new Action<bool>(state => Debug.LogWarning("[UISyncToggle] Toggle initiated, but no callback defined. Inherit this class and define your custom callbacks properly."));
            toggle.onValueChanged.AddListener(state => selfToggleAction(state));
            var toggleable = target.GetComponent<IToggleable>();
            if (toggleable == null)
            {
                Debug.LogWarning("[UISyncToggle] " + target.name + " is marked as the target of this toggle, but it does not have a script that implements IToggleable on it!");
            }
            else
            {
                toggleable.Toggled += (sender, args) =>
                {
                    toggle.isOn = args.value;
                    targetToggleAction = targetToggleAction ?? new Action<bool>(state => Debug.LogWarning("[UISyncToggle] Toggle initiated, but no callback defined. Inherit this class and define your custom callbacks properly."));
                    targetToggleAction(toggle.isOn);
                };
            }
        }
    }
}

