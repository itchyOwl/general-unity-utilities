using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using ItchyOwl.DataManagement;

namespace ItchyOwl.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Copies all fields and properties of the source to the destination.
        /// Usage: myComp.CopyValuesOf(someOtherComponent).
        /// Note: In editor scripts, use EditorUtility.CopySerialized(source, target) instead.
        /// Note that this method may cause unecessary material instances to be instantiated, where we would like to use a shared materials instead.
        /// </summary>
        public static T CopyValuesOf<T>(this Component comp, T source) where T : Component
        {
            return FileManager.CopyValuesOf(source, comp as T);
        }

        #region Hierarchy
        /// <summary>
        /// Returns all children. A short hand for component extension GetComponentsOnlyInChildren<Transform>(true).
        /// </summary>
        public static IEnumerable<Transform> GetAllChildren(this Component c)
        {
            return c.GetComponentsOnlyInChildren<Transform>(true);
        }

        /// <summary>
        /// Filters duplicates.
        /// </summary>
        public static IEnumerable<T> GetComponentsInChildrenAndParents<T>(this Component c, bool includeInactive = false)
        {
            var children = c.GetComponentsInChildren<T>(includeInactive);
            var parents = c.GetComponentsOnlyInParents<T>(includeInactive);
            return children.Concat(parents);
        }

        /// <summary>
        /// Finds and returns all components of type T that are found under this game object excluding the components found on the caller itself.
        /// </summary>
        public static IEnumerable<T> GetComponentsOnlyInChildren<T>(this Component c, bool includeInactive = false)
        {
            var components = c.GetComponents<T>();
            return c.GetComponentsInChildren<T>(includeInactive).Where(obj => !components.Contains(obj));
        }

        /// <summary>
        /// Finds and returns all components of type T that are found from parents excluding the components found on the caller itself.
        /// </summary>
        public static IEnumerable<T> GetComponentsOnlyInParents<T>(this Component c, bool includeInactive = false)
        {
            var components = c.GetComponents<T>();
            return c.GetComponentsInParent<T>(includeInactive).Where(obj => !components.Contains(obj));
        }

        /// <summary>
        /// Removes all components of type T found from the children of this component.
        /// </summary>
        /// <returns>Returns the count of removed components.</returns>
        public static int RemoveComponentsInChildren<T>(this Component c, bool isUsedInEditor = false) where T : Component
        {
            int counter = 0;
            foreach (var transform in c.GetComponentsInChildren<Transform>(true))
            {
                var components = transform.GetComponents<T>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (isUsedInEditor) { Object.DestroyImmediate(components[i]); }
                    else { Object.Destroy(components[i]); }
                    counter++;
                }
            }
            return counter;
        }
        #endregion
    }
}

