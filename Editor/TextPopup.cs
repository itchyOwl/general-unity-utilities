using UnityEngine;
using UnityEditor;

namespace ItchyOwl.Editor
{
    /// <summary>
    /// Display a popup window with a text and a "OK" button to close the window.
    /// </summary>
    public class TextPopup : EditorWindow
    {
        private static string text;

        public static void Display(string msg)
        {
            text = msg;
            TextPopup window = GetWindow<TextPopup>(true, "Notification");
            window.ShowAuxWindow();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(text, EditorStyles.wordWrappedLabel);
            GUILayout.Space(70);
            if (GUILayout.Button("OK"))
            {
                Close();
            }
        }
    }
}
