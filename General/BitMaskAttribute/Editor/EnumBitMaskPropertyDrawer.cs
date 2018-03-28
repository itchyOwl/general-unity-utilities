using UnityEditor;
using UnityEngine;

namespace ItchyOwl.General.Editor
{
    // http://answers.unity3d.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html
    [CustomPropertyDrawer(typeof(BitMaskAttribute))]
    public class EnumBitMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var typeAttr = attribute as BitMaskAttribute;
            // Add the actual int value behind the field name
            label.text = label.text + "(" + prop.intValue + ")";
            prop.intValue = EditorUtilities.DrawBitMaskField(position, prop.intValue, typeAttr.propType, label);
        }
    }
}