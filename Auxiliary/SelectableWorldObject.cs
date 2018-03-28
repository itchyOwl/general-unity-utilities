using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Used for storing the selection state per object.
    /// The Selector logic is usually app specific, but the methods for selecting are found in WorldObjectSelector.
    /// </summary>
    public class SelectableWorldObject : MonoBehaviour
    {
        public bool IsHighlighted { get; private set; }
        public bool IsSelected { get; private set; }

        public virtual void Highlight(bool enabled)
        {
            if (IsHighlighted == enabled) { return; }
            IsHighlighted = enabled;
        }

        public virtual void Select(bool enabled)
        {
            if (IsSelected == enabled) { return; }
            IsSelected = enabled;
        }
    }
}

