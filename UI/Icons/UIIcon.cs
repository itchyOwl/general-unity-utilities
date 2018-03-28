using UnityEngine;
using UnityEngine.UI;
using ItchyOwl.Extensions;

namespace ItchyOwl.UI
{
    /// <summary>
    /// Base class for all interactable icons.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIIcon : MonoBehaviour
    {
        private Button _button;
        /// <summary>
        /// Cannot return null.
        /// </summary>
        public Button Button
        {
            get
            {
                _button = gameObject.GetOrAddComponent<Button>();
                return _button;
            }
        }

        [SerializeField]
        private Image _image;
        /// <summary>
        /// May return null
        /// </summary>
        public Image Image
        {
            get
            {
                _image = gameObject.GetReferenceTo(_image, includeInactive: true, seekChildren: true);
                return _image;
            }
        }

        [SerializeField]
        private Text _text;
        /// <summary>
        /// May return null.
        /// </summary>
        public Text Text
        {
            get
            {
                _text = gameObject.GetReferenceTo(_text, includeInactive: true, seekChildren: true);
                return _text;
            }
        }
    }
}

