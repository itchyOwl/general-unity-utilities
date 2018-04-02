using System.Linq;
using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// An abstract base class for all the scriptable objects that should only have a single instance.
    /// Implements a static Instance property.
    /// NOTE:
    /// This class only handles the loading of the instance.
    /// No instances are automatically created or destroyed.
    /// An error is thrown if no instance is found or if there are multiple, but the single instance pattern is not in any way forced, because there are multiple ways to create scriptable objects.
    /// </summary>
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var type = typeof(T);
                    var instances = Resources.LoadAll<T>(string.Empty);
                    _instance = instances.FirstOrDefault();
                    if (_instance == null)
                    {
                        Debug.LogErrorFormat("[ScriptableSingleton] No instance of {0} found!", type.ToString());
                    }
                    else if (instances.Multiple())
                    {
                        Debug.LogErrorFormat("[ScriptableSingleton] Multiple instances of {0} found!", type.ToString());
                    }
                    else
                    {
                        Debug.LogFormat("[ScriptableSingleton] An instance of {0} was found!", type.ToString());
                    }
                }
                return _instance;
            }
        }
    }
}
