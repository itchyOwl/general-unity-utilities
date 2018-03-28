using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace ItchyOwl.UI
{
    /// <summary>
    /// More specific type of UIElementFactory, where the elements are stored in a dictionary.
    /// </summary>
    public class UIDictionaryElementFactory<T> : UIElementFactory<T> where T : MonoBehaviour
    {
        private Dictionary<object, T> elementDict = new Dictionary<object, T>();

        public bool HasElement(object key)
        {
            return elementDict.ContainsKey(key);
        }

        public bool TryGetElement(object key, out T element)
        {
            return elementDict.TryGetValue(key, out element);
        }

        /// <summary>
        /// All methods are launched in order and only when the element is created and initialized. Configuration Method is the first to launch, callback is the last.
        /// </summary>
        public T GetOrCreateElement(object key, Action<T> configurationMethod = null, Func<T, string> namingFunction = null, Func<T, string> textElementFunction = null, Action<T> callback = null)
        {
            T element;
            if (!elementDict.TryGetValue(key, out element))
            {
                element = CreateDefaultElement().GetComponentInChildren<T>();
                elementDict.Add(key, element);
                if (configurationMethod != null)
                {
                    configurationMethod(element);
                }
                if (namingFunction != null)
                {
                    element.name = namingFunction(element);
                }
                if (textElementFunction != null)
                {
                    var tmpTextComponent = element.GetComponentInChildren<Text>(includeInactive: false);
                    if (tmpTextComponent != null)
                    {
                        tmpTextComponent.text = textElementFunction(element);
                    }
                    else
                    {
                        var unityTextComponent = element.GetComponentInChildren<Text>(includeInactive: false);
                        if (unityTextComponent != null)
                        {
                            unityTextComponent.text = textElementFunction(element);
                        }
                    }
                }
                if (callback != null)
                {
                    callback(element);
                }
            }
            return element;
        }

        public bool TryDestroyElement(object key)
        {
            T element;
            if (elementDict.TryGetValue(key, out element))
            {
                DestroyElement(element);
                elementDict.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void ClearAll()
        {
            base.ClearAll();
            elementDict.Clear();
        }
    }
}
