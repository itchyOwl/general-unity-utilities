using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ItchyOwl.Editor
{
    public class ScriptableObjectWindow : EditorWindow
    {
        private int selectedIndex;
        private string[] names;
        private Type[] types;

        public void SetTypes(Type[] types)
        {
            this.types = types;
            names = types.Select(t => t.FullName).ToArray();
        }

        public IEnumerable<Type> GetTypes() { return types; }

        private void OnGUI()
        {
            GUILayout.Label("ScriptableObject Class");
            selectedIndex = EditorGUILayout.Popup(selectedIndex, names);
            if (GUILayout.Button("Create"))
            {
                if (types.Length == 0) { return; }
                ScriptableObjectFactory.Create(types[selectedIndex], names[selectedIndex]);
                Close();
            }
        }
    }
}
