using UnityEngine;
using System.Collections.Generic;
using System;
using ItchyOwl.General;
using ItchyOwl.Extensions;

namespace ItchyOwl.UI
{
    /// <summary>
    /// Creates UI elements and positions them as childs of this transform.
    /// Can be used for example in scrollable lists.
    /// Handles only game objects. Use generic version if you have a specific type.
    /// Note: Cannot use the generic version for game objects, because GameObject and Component are siblings and their base class Object cannot access the transform component.
    /// In order to use the generic version, you have to create a custom class that inherits it, which in some cases may be too much of an overhead.
    /// </summary>
    public class UIElementFactory : MonoBehaviour
    {
        public GameObject defaultPrefab;

        public event EventHandler<EventArg<GameObject>> ElementCreated;
        public event EventHandler<EventArg<GameObject>> ElementDestroyed;

        protected HashSet<GameObject> elements = new HashSet<GameObject>();
        public HashSet<GameObject> Elements { get { return elements; } }

        public virtual GameObject CreateDefaultElement()
        {
            return CreateElement(defaultPrefab);
        }

        public virtual GameObject CreateElement(GameObject go)
        {
            GameObject element = Instantiate(go);
            element.transform.SetParent(transform, worldPositionStays: false);
            elements.Add(element);
            if (ElementCreated != null)
            {
                ElementCreated(this, new EventArg<GameObject>(element));
            }
            return element;
        }

        public virtual bool DestroyElement(GameObject element)
        {
            if (elements.Contains(element))
            {
                elements.Remove(element);
                Destroy(element);
                if (ElementDestroyed != null)
                {
                    ElementDestroyed(this, new EventArg<GameObject>(element));
                }
                return true;
            }
            else
            {
                Debug.LogWarningFormat("[UIElementFactory] {0} was not created by this factory!", element.name);
                return false;
            }
        }

        public virtual void ClearAll()
        {
            elements.ForEachMod(e => DestroyElement(e));
        }
    }

    /// <summary>
    /// Creates UI elements and positions them as childs of this transform.
    /// Can be used for example in scrollable lists.
    /// A generic version.
    /// </summary>
    public class UIElementFactory<T> : MonoBehaviour where T : Component
    {
        public T defaultPrefab;

        public event EventHandler<EventArg<T>> ElementCreated;
        public event EventHandler<EventArg<T>> ElementDestroyed;

        protected HashSet<T> elements = new HashSet<T>();
        public HashSet<T> Elements { get { return elements; } }

        public virtual T CreateDefaultElement()
        {
            return CreateElement(defaultPrefab);
        }

        public virtual T CreateElement(T prefab)
        {
            T element = Instantiate(prefab);
            element.transform.SetParent(transform, worldPositionStays: false);
            elements.Add(element);
            if (ElementCreated != null)
            {
                ElementCreated(this, new EventArg<T>(element));
            }
            return element;
        }

        public virtual bool DestroyElement(T element)
        {
            if (elements.Contains(element))
            {
                elements.Remove(element);
                Destroy(element.gameObject);
                if (ElementDestroyed != null)
                {
                    ElementDestroyed(this, new EventArg<T>(element));
                }
                return true;
            }
            else
            {
                Debug.LogWarningFormat("[UIElementFactory] {0} was not created by this factory!", element.name);
                return false;
            }
        }

        public virtual void ClearAll()
        {
            elements.ForEachMod(e => DestroyElement(e));
        }
    }
}

