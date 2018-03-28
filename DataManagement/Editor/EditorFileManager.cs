using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.DataManagement.Editor
{
    public static class EditorFileManager
    {
        /// <summary>
        /// UnityEngine.LoadAssetsAtPath does not load multiple files. It loads multiple sub assets if the main asset has many. Moreover, it's not generic. Therefore this method.
        /// Note: Does not work for sub assets. For loading sub assets (like procedural materials of a substance archive), use UnityEngine.LoadAssetsAtPath.
        /// Use in the editor only.
        /// </summary>
        public static List<T> LoadAssetsAtPath<T>(string path) where T : UnityEngine.Object
        {
            path = HandleAssetPath(path);
            Debug.Log("[EditorFileManager] Loading assets from " + path);
            var filePaths = Directory.GetFiles(path);
            var filteredPaths = filePaths.Where(p => !p.Contains(".meta"));
            List<T> assets = new List<T>();
            foreach (var p in filteredPaths)
            {
                var assetPath = FileManager.GetRightPartOfPath(p, "Assets", '/');
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            Debug.Log("[EditorFileManager] Assets found: " + assets.Count);
            return assets;
        }

        /// <summary>
        /// Method for editor scripts. TODO: remove?
        /// </summary>
        public static T CreateScriptableObject<T>(string fileName, string path) where T : ScriptableObject
        {
            var paths = FileManager.SeparateFileNameFromPath(fileName, path, '/');
            if (!path.Contains("Assets"))
            {
                Debug.Log("[EditorFileManager] adding 'Assets' to the path.");
                paths.pathWithoutFileName = Path.Combine("Assets/", paths.pathWithoutFileName);
                paths.fullPath = Path.Combine("Assets/", paths.fullPath);
            }
            Debug.Log("[EditorFileManager] Checking path: " + paths.pathWithoutFileName);
            if (!AssetDatabase.IsValidFolder(paths.pathWithoutFileName))
            {
                Debug.Log("[EditorFileManager] The given path does not exists. Starting recursive path check...");
                paths.pathWithoutFileName = CreatePath(paths.pathWithoutFileName);
            }
            T instance = ScriptableObject.CreateInstance<T>();
            instance.name = fileName;
            paths.fullPath = AssetDatabase.GenerateUniqueAssetPath(paths.fullPath + ".asset");
            AssetDatabase.CreateAsset(instance, paths.fullPath);
            Debug.LogFormat("[EditorFileManager] {0} created in {1}", instance.name, paths.pathWithoutFileName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = instance;
            return instance;
        }

        /// <summary>
        /// Method for editor scripts.
        /// </summary>
        public static string CreatePath(string path)
        {
            if (!path.Contains("Assets"))
            {
                Debug.Log("[EditorFileManager] adding 'Assets' to the path.");
                path = Path.Combine("Assets/", path);
            }
            Debug.Log("[EditorFileManager] Creating path: " + path);
            var parsedPath = path.Split(new string[] { "/" }, StringSplitOptions.None);
            var parents = new List<string>();
            parsedPath = parsedPath.Where(p => !string.IsNullOrEmpty(p)).ToArray();
            for (int i = 0; i < parsedPath.Length; i++)
            {
                string folderName = parsedPath[i];
                Debug.Log("[EditorFileManager] Checking folder: " + folderName);
                if (folderName == "Assets")
                {
                    Debug.Log("[EditorFileManager] At the project root...");
                    continue;
                }
                if (i == 1)
                {
                    if (parsedPath.Length == 2)
                    {
                        if (!AssetDatabase.IsValidFolder("Assets/" + folderName))
                        {
                            Debug.LogFormat("[EditorFileManager] Creating folder {0} in {1}", folderName, "Assets");
                            AssetDatabase.CreateFolder("Assets", folderName);
                        }
                    }
                    continue;
                }
                string parentFolder = parsedPath[i - 1];
                parents.Add(parentFolder);
                string p2 = string.Join("/", parents.ToArray()) + "/";
                if (!AssetDatabase.IsValidFolder(p2))
                {
                    Debug.LogFormat("[EditorFileManager] Creating folder {0} in {1}", folderName, p2);
                    AssetDatabase.CreateFolder(p2, folderName);
                }
            }
            if (!AssetDatabase.IsValidFolder(path))
            {
                throw new Exception("[EditorFileManager] Failed to create path: " + path);
            }
            return path;
        }

        private static string HandleAssetPath(string path)
        {
            var parts = path.Split('/').Where(p => !p.IsNullOrEmpty()).ToList();
            var firstPart = parts.First();
            if (firstPart.ToLowerInvariant().Equals("assets"))
            {
                parts.Remove(firstPart);
            }
            path = string.Join("/", parts.ToArray());
            if (path[0] != '/')
            {
                path = "/" + path;
            }
            return Application.dataPath + path;
        }
    }
}
