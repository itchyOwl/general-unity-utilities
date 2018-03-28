using UnityEngine;
using UnityEngine.UI;
using ItchyOwl.Extensions;

namespace ItchyOwl.UI
{
    [RequireComponent(typeof(Button))]
    public class UIWindowActionButton : MonoBehaviour
    {
        public UIWindow target;
        public UIWindowAction.WindowAction action;
        private Button _button;
        public Button Button
        {
            get
            {
                _button = gameObject.GetReferenceTo(_button);
                return _button;
            }
        }

        public UIWindowAction Action
        {
            get
            {
                if (_action == null)
                {
                    _action = new UIWindowAction(action, target);
                }
                return _action;
            }
        }
        private UIWindowAction _action;

        protected virtual void Awake()
        {
            Button.onClick.AddListener(() => Action.Execute());
        }

        /// <summary>
        /// Removes all listeners and re-adds the pre-defined actions.
        /// </summary>
        public virtual void ResetListeners()
        {
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => Action.Execute());
        }
    }
}
