using UnityEngine;

namespace ItchyOwl.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Gets or adds a component.
        /// Note that this only returns the first component if there are many.
        /// By default does not seek children, but can be told to do so.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject go, bool seekChildren = false) where T : Component
        {
            T component = seekChildren ? go.GetComponentInChildren<T>(includeInactive: true) : go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Checks if the instance is null, and if it is, seeks for the component.
        /// By default does not seek children or parents, but can be told to do so.
        /// </summary>
        public static T GetReferenceTo<T>(this GameObject go, T instance, bool includeInactive = true, bool seekChildren = false, bool seekParents = false)
        {
            if (instance == null)
            {
                instance = seekChildren ? go.GetComponentInChildren<T>(includeInactive) : go.GetComponent<T>();
                if (instance == null && seekParents)
                {
                    instance = go.GetComponentInParent<T>();
                }
            }
            return instance;
        }

        /// <summary>
        /// Adds a copy of the specified component with all public fields and properties copied. Private fields and properties are not copied.
        /// Usage: Health myHealth = gameObject.AddComponent<Health>(enemy.health);
        /// </summary>
        /// <source>http://answers.unity3d.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html</source>
        public static T AddComponentCopy<T>(this GameObject go, T original) where T : Component
        {
            return go.AddComponent<T>().CopyValuesOf(original);
        }

        /// <summary>
        /// Removes all components of type T found from this game object.
        /// </summary>
        /// <returns>Returns the count of removed components.</returns>
        public static int RemoveComponents<T>(this GameObject go, bool isUsedInEditor = false, bool includingChildren = false) where T : Component
        {
            if (includingChildren)
            {
                return RemoveComponentsInChildren<T>(go, isUsedInEditor);
            }
            else
            {
                int counter = 0;
                var components = go.GetComponents<T>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (isUsedInEditor) { Object.DestroyImmediate(components[i]); }
                    else { Object.Destroy(components[i]); }
                    counter++;
                }
                return counter;
            }
        }

        /// <summary>
        /// Removes all components of type T found from the children of this game object.
        /// </summary>
        /// <returns>Returns the count of removed components.</returns>
        public static int RemoveComponentsInChildren<T>(this GameObject go, bool isUsedInEditor = false) where T : Component
        {
            return go.transform.RemoveComponentsInChildren<T>(isUsedInEditor);
        }
    }
}
