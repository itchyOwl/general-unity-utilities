using UnityEngine;
using System.Collections.Generic;
using ItchyOwl.General;

namespace ItchyOwl.UI
{
    /// <summary>
    /// Seeks IToggleables from the targets and synchronizes their toggle calls.
    /// </summary>
    public class UIToggleSync : MonoBehaviour
    {
        /// <summary>
        /// Cannot serialize interfaces, thus GameObject is used.
        /// </summary>
        public List<GameObject> targets = new List<GameObject>();

        private void Awake()
        {
            var myToggle = GetComponent<IToggleable>();
            myToggle.Toggled += (sender, args) =>
            {
                foreach (var target in targets)
                {
                    var toggleTargets = target.GetComponentsInChildren<IToggleable>();
                    foreach (var toggleTarget in toggleTargets)
                    {
                        toggleTarget.Toggle();
                    }
                }
            };
        }
    }
}
