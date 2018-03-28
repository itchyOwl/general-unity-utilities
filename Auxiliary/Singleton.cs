using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// An abstract base class for singletons that by default live in a scene. Use DontDestroyOnLoad if you want a persistent singleton.
    /// The purpose of this class is to reduce the boiler-plate code related to singletons.
    /// Note that you don't have to use this class, if you need to inherit some other class. You can use the Component.GetSingleton() method instead and implement your own pattern.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static string debugTag = string.Format("[Singleton {0}]", typeof(T).ToString());

        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (isQuitting) { return _instance; }
                _instance = _instance.GetSingleton(typeof(T).ToString());
                return _instance;
            }
        }

        /// <summary>
        /// Attempts to cast the instance to a derivative type of T.
        /// If that cannot be done, gets a new instance and assings that as singleton.
        /// If the instance is upgraded, the old instance is destroyed.
        /// </summary>
        /// <typeparam name="TDerivate">More specific type of T. Derivative of T.</typeparam>
        /// <returns>Returns the instance as TDerivative</returns>
        public static TDerivate GetInstance<TDerivate>() where TDerivate : T
        {
            var i = _instance as TDerivate;
            if (isQuitting) { return i; }
            string derivateTypeName = typeof(TDerivate).ToString();
            i = i.GetSingleton(derivateTypeName);
            if (_instance != null && i != _instance)
            {
                Debug.LogFormat("{0} Upgrading the old instance to {1}", debugTag, derivateTypeName);
                i.CopyValuesOf(_instance);
                Debug.LogFormat("{0} Destroying the old instance of type {1}", debugTag, derivateTypeName);
                Destroy(_instance.gameObject);
            }
            _instance = i;
            return i;
        }

        protected static bool isQuitting;
        protected virtual void OnApplicationQuit()
        {
            isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (isQuitting) { return; }
            if (_instance == this)
            {
                _instance = null;
            }
        }

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            if (IsDuplicate(this))
            {
                Debug.LogWarningFormat("[Singleton {0}] Another instance already found! Destroying self as a duplicate.", typeof(T).ToString());
                Destroy(this);
            }
        }

        protected static bool IsDuplicate(Singleton<T> instance)
        {
            if (instance == null) { return false; }
            if (_instance == null) { return false; }
            return _instance != instance;
        }
    }
}
