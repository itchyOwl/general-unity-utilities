using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ItchyOwl.DataManagement.Editor;

namespace ItchyOwl.Auxiliary.Editor
{
    public class TextureMapper : EditorWindow
    {
        [Serializable]
        private class Link
        {
            public string textureKeyword;
            public string texturePropertyName;

            public Link(string textureKeyword, string texturePropertyName)
            {
                this.textureKeyword = textureKeyword;
                this.texturePropertyName = texturePropertyName;
            }
        }

        [SerializeField]
        private static string materialsPath = "Models/Materials/";
        [SerializeField]
        private static string texturesPath = "Models/Textures/";
        [SerializeField]
        private static string separator = "_";
        [SerializeField]
        private static List<Link> links = new List<Link>()
        {
            new Link("Albedo", "_MainTex"),
            new Link("Normal", "_BumpMap"),
            new Link("MetallicSmoothness", "_MetallicGlossMap")
        };

        #region Mapping
        public static void MapTexturesToMaterials()
        {
            // TODO: enable searching all materials and textures in the project
            var materials = EditorFileManager.LoadAssetsAtPath<Material>(materialsPath);
            var textures = EditorFileManager.LoadAssetsAtPath<Texture>(texturesPath);
            foreach (var material in materials)
            {
                links.ForEach(link => TryMapTexture(material, textures, link.textureKeyword, link.texturePropertyName));
                // TODO: if cannot find a normal map with string matching, compare the type?
            }
        }

        private static Texture FindTexture(List<Texture> textures, Material material, string keyword)
        {
            return textures.Where(t => t.name.StartsWith(material.name + separator + keyword.ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        private static bool TryMapTexture(Material material, List<Texture> textures, string keyword, string texturePropertyName)
        {
            var texture = FindTexture(textures, material, keyword);
            if (texture != null)
            {
                Debug.LogFormat("[TextureMapper] Found a texture ({0}) with the keyword '{1}'. Setting it on the material '{2}'", texture.name, keyword, material.name);
                material.SetTexture(texturePropertyName, texture);
            }
            else
            {
                Debug.LogWarningFormat("[TextureMapper] Could not find a texture with the keyword '{0}' for {1}", keyword, material.name);
            }
            return texture != null;
        }
        #endregion

        #region GUI
        private static float windowWidth = 800;
        private static float windowHeight = 500;
        private static float leftMargin = 50;
        private static float topMargin = 150;

        [MenuItem("Assets/TextureMapper")]
        public static void OpenTextureMapper()
        {
            var window = GetWindow<TextureMapper>(utility: true, title: "TextureMapper", focus: true);
            window.position = new Rect(leftMargin, topMargin, windowWidth, windowHeight);
            window.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Path to materials:");
            materialsPath = EditorGUILayout.TextField(materialsPath);
            EditorGUILayout.LabelField("Path to textures:");
            texturesPath = EditorGUILayout.TextField(texturesPath);
            EditorGUILayout.LabelField("Separator character (used to discern the name of the subject from the rest of the file name):");
            separator = EditorGUILayout.TextField(separator);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            for (int i = 0; i < links.Count; i++)
            {
                var link = links[i];
                EditorGUILayout.LabelField(string.Format("Link {0}:", i));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Texture keyword:", new GUILayoutOption[] { GUILayout.Width(120) });
                link.textureKeyword = EditorGUILayout.TextField(link.textureKeyword);
                GUILayout.Space(50);
                EditorGUILayout.LabelField("Texture property name:", new GUILayoutOption[] { GUILayout.Width(150) });
                link.texturePropertyName = EditorGUILayout.TextField(link.texturePropertyName);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(30) }))
            {
                links.Add(new Link("new keyword", "new property name"));
            }
            if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(30) }))
            {
                links.Remove(links.Last());
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 50));
            if (GUILayout.Button("Map textures to materials", new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(50) }))
            {
                MapTexturesToMaterials();
            }
            GUILayout.EndArea();
        }
        #endregion
    }
}
