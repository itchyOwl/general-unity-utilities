using System;
using UnityEngine;

namespace ItchyOwl.General.Editor
{
    // http://answers.unity3d.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html
    public class BitMaskAttribute : PropertyAttribute
    {
        public Type propType;
        public BitMaskAttribute(Type aType)
        {
            propType = aType;
        }
    }
}