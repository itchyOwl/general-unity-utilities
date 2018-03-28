using UnityEngine;
using System;
using System.Collections.Generic;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Selection controller class that works together with SelectableWorldObjects.
    /// Inherit and implement custom selection logic. Can also be used from outside, if that's preferred.
    /// </summary>
    public class WorldObjectSelector : Singleton<WorldObjectSelector>
    {
        public static event Action<SelectableWorldObject> ObjectHighlighted = obj => Debug.Log("[WorldObjectSelector] " + obj.name + " highlighted");
        public static event Action<SelectableWorldObject> ObjectUnhighlighted = obj => Debug.Log("[WorldObjectSelector] " + obj.name + " unhighlighted");
        public static event Action<SelectableWorldObject> ObjectSelected = obj => Debug.Log("[WorldObjectSelector] " + obj.name + " selected");
        public static event Action<SelectableWorldObject> ObjectDeselected = obj => Debug.Log("[WorldObjectSelector] " + obj.name + " deselected");

        protected HashSet<SelectableWorldObject> highlightedObjects = new HashSet<SelectableWorldObject>();
        protected HashSet<SelectableWorldObject> selectedObjects = new HashSet<SelectableWorldObject>();

        public void ToggleSelect(SelectableWorldObject target)
        {
            if (target != null)
            {
                if (target.IsSelected) { Deselect(target); }
                else { Select(target); }
            }
        }

        public void Select(SelectableWorldObject target)
        {
            target.Select(true);
            selectedObjects.Add(target);
            ObjectSelected(target);
        }

        public void Deselect(SelectableWorldObject target)
        {
            target.Select(false);
            selectedObjects.Remove(target);
            ObjectDeselected(target);
        }

        public void Highlight(SelectableWorldObject target)
        {
            if (!target.IsHighlighted)
            {
                highlightedObjects.Add(target);
                target.Highlight(true);
                ObjectHighlighted(target);
            }
        }

        public void Unhighlight(SelectableWorldObject target)
        {
            if (target.IsHighlighted)
            {
                highlightedObjects.Remove(target);
                target.Highlight(false);
                ObjectUnhighlighted(target);
            }
        }

        public void DeselectAll()
        {
            Deselect(selectedObjects);
        }

        public void UnhighlightAll()
        {
            Unhighlight(highlightedObjects);
        }

        protected void Select(IEnumerable<SelectableWorldObject> targets)
        {
            targets.ForEachMod(t => Select(t));
        }

        protected void Deselect(IEnumerable<SelectableWorldObject> targets)
        {
            targets.ForEachMod(t => Deselect(t));
        }

        protected void Highlight(IEnumerable<SelectableWorldObject> targets)
        {
            targets.ForEachMod(t => Highlight(t));
        }

        protected void Unhighlight(IEnumerable<SelectableWorldObject> targets)
        {
            targets.ForEachMod(t => Unhighlight(t));
        }

        protected void ToggleSelect(IEnumerable<SelectableWorldObject> targets)
        {
            targets.ForEachMod(t => ToggleSelect(t));
        }
    }
}
