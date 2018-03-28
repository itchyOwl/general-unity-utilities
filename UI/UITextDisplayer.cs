using UnityEngine;
using UnityEngine.UI;
using ItchyOwl.Extensions;

namespace ItchyOwl.UI
{
    public abstract class UITextDisplayer<T> : MonoBehaviour
    {
        private Text _textComponent;
        public Text TextComponent
        {
            get
            {
                _textComponent = gameObject.GetReferenceTo(_textComponent, includeInactive: true, seekChildren: true);
                return _textComponent;
            }
        }

        public T CurrentSource { get; set; }

        protected virtual void Awake()
        {
            if (CurrentSource == null)
            {
                Clear();
            }
        }

        /// <summary>
        /// Assigns the data as the current source.
        /// Override and implement this method in order to display the text.
        /// Remember to assign the text to the text component, like this: TextComponent.text = text;
        /// </summary>
        public virtual string Display(T data)
        {
            CurrentSource = data;
            return string.Empty;
        }

        public virtual void Clear()
        {
            TextComponent.text = string.Empty;
        }
    }
}

