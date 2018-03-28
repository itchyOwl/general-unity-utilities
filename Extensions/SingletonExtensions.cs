using UnityEngine;
using System.Linq;

namespace ItchyOwl.Extensions
{
    /// <summary>
    /// MonoBehaviour extensions for implementing a singleton pattern.
    /// </summary>
    public static class SingletonExtensions
    {
        /// <summary>
        /// Checks the instance, and if no instance is found, instantiates a prefab and creates a singleton instance of it.
        /// NOTE: This method only handles the static instance reference. In order to fully implement the singleton pattern, you still have to check elsewhere that no instances of same type are instantiated.
        /// An error is thrown, if multiple instances are found in the scene, however.
        /// Note also that if this method is used in lazy evaluation pattern, it is possible that an instance is created when the application quits. In order to prevent this, you should call this method only when the application is not quitting.
        /// </summary>
        public static T GetSingleton<T>(this T instance, GameObject prefab) where T : MonoBehaviour
        {
            instance = GetInstance(instance);
            if (instance == null)
            {
                var go = Object.Instantiate(prefab);
                instance = go.GetOrAddComponent<T>(seekChildren: true);
                Debug.LogFormat("[SingletonExtensions] Cannot find an instance of {0}. An instance created under {1}", instance.GetType().ToString(), go.name);
            }
            return instance;
        }

        /// <summary>
        /// Checks the instance, and if no instance is found, creates a new game object, adds the component to it, and stores the singleton instance reference.
        /// NOTE: This method only handles the static instance reference. In order to fully implement the singleton pattern, you still have to check elsewhere that no instances of same type are instantiated.
        /// A warning is shown, if multiple instances are found in the scene, however.
        /// Note also that if this method is used in lazy evaluation pattern, it is possible that an instance is created when the application quits. In order to prevent this, you should call this method only when the application is not quitting.
        /// </summary>
        public static T GetSingleton<T>(this T instance, string name) where T : MonoBehaviour
        {
            instance = GetInstance(instance);
            if (instance == null)
            {
                instance = new GameObject(name).AddComponent<T>();
                Debug.LogFormat("[SingletonExtensions] Couldn't find an instance of {0}. An instance created under {1}", instance.GetType().ToString(), instance.name);
            }
            return instance;
        }

        /// <summary>
        /// Note: FindObjectsOfType cannot find objects that have not yet executed their Start method!
        /// </summary>
        private static T GetInstance<T>(T instance) where T : MonoBehaviour
        {
            if (instance == null)
            {
                var instances = Object.FindObjectsOfType<T>();
                instance = instances.FirstOrDefault();
                if (instance != null)
                {
                    Debug.LogFormat("[SingletonExtensions] Instance of {0} found in the scene from the object {1}.", instance.GetType().ToString(), instance.name);
                }
                if (instances.Multiple())
                {
                    Debug.LogWarningFormat("[SingletonExtensions] Multiple instances of {0} found!", instance.GetType().ToString());
                }
            }
            return instance;
        }
    }
}

