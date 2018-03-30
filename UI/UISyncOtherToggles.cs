using UnityEngine;
using System.Collections.Generic;
using ItchyOwl.General;

namespace ItchyOwl.UI
{
    /// <summary>
    /// Seeks IToggleables from the targets and synchronizes their toggle calls.
    /// Warning: Don't use to cross sync the targets, because that will cause an infinite loop.
    /// </summary>
    public class UISyncOtherToggles : MonoBehaviour
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
