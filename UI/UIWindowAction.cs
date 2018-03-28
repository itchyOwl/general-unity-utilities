using UnityEngine;
using System;

namespace ItchyOwl.UI
{
    public class UIWindowAction : IUIAction
    {
        public enum WindowAction
        {
            None,
            OpenSingle,
            OpenAdditive,
            Close,
            Toggle,
            CloseAll,
            OpenRecentlyClosed,
            OpenOnlyRecentlyClosed,
            OpenPreviouslyOpen,
            OpenOnlyPreviouslyOpen
        }
        public WindowAction actionType = WindowAction.None;
        public UIWindow target;

        private Action<UIWindow> action = w => Debug.LogWarning("[UIWindowAction] Action not defined!");

        public UIWindowAction(WindowAction actionType, UIWindow target)
        {
            this.target = target;
            DetermineAction(actionType);
        }

        public void Execute()
        {
            action(target);
        }

        private void DetermineAction(WindowAction actionType)
        {
            switch (actionType)
            {
                case WindowAction.CloseAll:
                    action = w => GUIManager.CloseAllWindows();
                    break;
                case WindowAction.OpenAdditive:
                    action = w => GUIManager.OpenWindow(w);
                    break;
                case WindowAction.OpenSingle:
                    action = w => GUIManager.OpenOnlyWindow(w);
                    break;
                case WindowAction.OpenRecentlyClosed:
                    action = w => GUIManager.OpenRecentlyClosedWindows();
                    break;
                case WindowAction.OpenOnlyRecentlyClosed:
                    action = w => GUIManager.OpenOnlyRecentlyClosedWindows();
                    break;
                case WindowAction.OpenPreviouslyOpen:
                    action = w => GUIManager.OpenPreviouslyOpenWindows();
                    break;
                case WindowAction.OpenOnlyPreviouslyOpen:
                    action = w => GUIManager.OpenOnlyPreviouslyOpenWindows();
                    break;
                case WindowAction.Close:
                    action = w => GUIManager.CloseWindow(w);
                    break;
                case WindowAction.Toggle:
                    action = w => GUIManager.ToggleWindow(w);
                    break;
                case WindowAction.None:
                    action = w => { };
                    break;
                default: throw new NotImplementedException(actionType.ToString());
            }
        }
    }
}
