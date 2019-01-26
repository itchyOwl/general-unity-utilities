using ItchyOwl.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ItchyOwl.DataManagement
{
    public static class FileManager
    {
        #region Main methods
        public static bool SaveAsText(string source, string path, string fileName, bool isPersistentPath, bool overWriteOldFile = false)
        {
            var paths = ParsePath(fileName, path, isPersistentPath);
            Debug.Log("[FileManager] Saving as text to " + paths.fullPath);
            if (File.Exists(paths.fullPath))
            {
                if (overWriteOldFile)
                {
                    Debug.Log("[FileManager] File already found, overwriting.");
                    File.WriteAllText(paths.fullPath, source);
                }
                else
                {
                    Debug.Log("[FileManager] File already found, appending.");
                    File.AppendAllText(paths.fullPath, source);
                }
                return true;
            }
            else
            {
                var dInfo = Directory.CreateDirectory(paths.pathWithoutFileName);
                if (dInfo.Exists)
                {
                    File.WriteAllText(paths.fullPath, source);
                    Debug.Log("[FileManager] File created at " + paths.fullPath);
                    return true;
                }
                else
                {
                    Debug.LogWarning("[FileManager] Cannot create directories for path: " + paths.pathWithoutFileName);
                    return false;
                }
            }
        }

        /// <summary>
        /// Creates a copy of the file and saves it into a file.
        /// </summary>
        public static bool Save<T>(T source, string path, string fileName, bool isPersistentPath, bool overWriteOldFile = false) where T : new()
        {
            var paths = ParsePath(fileName, path, isPersistentPath);
            Debug.Log("[FileManager] Saving as binary to " + paths.fullPath);
            if (!overWriteOldFile && File.Exists(paths.fullPath))
            {
                Debug.LogWarning("[FileManager] A file with the same path already exists. Overwriting is disabled.");
                return false;
            }
            else
            {
                var dInfo = Directory.CreateDirectory(paths.pathWithoutFileName);
                if (dInfo.Exists)
                {
                    using (var file = File.Create(paths.fullPath))
                    {
                        T copy = CreateCopy(source);
                        var bf = new BinaryFormatter();
                        // Throws a serialization exception, which could propably be avoided by using custom serializable classes instead of built-in structs. This is something I don't want to do.
                        // Use ToArray() extension methods and store the values as arrays instead.
                        //bf.SurrogateSelector = new UnityStructSurrogate();
                        try
                        {
                            bf.Serialize(file, copy);
                        }
                        catch (SerializationException e)
                        {
                            Debug.LogError("[FileManager] Cannot serialize: " + e);
                            return false;
                        }
                        Debug.Log("[FileManager] File created at " + paths.fullPath);
                        return true;
                    }
                }
                else
                {
                    Debug.LogError("[FileManager] Cannot create directories for path: " + paths.pathWithoutFileName);
                    return false;
                }
            }
        }

        public static T Load<T>(T classToLoadDataInto, string path, bool isPersistentPath) where T : new()
        {
            path = HandlePath(path, isPersistentPath);
            Debug.Log("[FileManager] loading from " + path);
            if (File.Exists(path))
            {
                try
                {
                    var bf = new BinaryFormatter();
                    using (var file = File.Open(path, FileMode.Open))
                    {
                        T data = (T)bf.Deserialize(file);
                        return data;
                    }
                }
                catch (SerializationException e)
                {
                    Debug.LogError("[FileManager] The file was found but the file could not be deserialized properly. This might be because the file was not in binary format or that the serialized class has been altered. Following SerializationException was thrown: " + e);
                    return default(T);
                }
            }
            else
            {
                Debug.Log("[FileManager] The file does not exist!");
                return default(T);
            }
        }

        public static string LoadText(string path, bool isPersistentPath)
        {
            path = HandlePath(path, isPersistentPath);
            Debug.Log("[FileManager] loading text file from " + path);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                Debug.Log("[FileManager] The file does not exist!");
                return string.Empty;
            }
        }

        public static bool DeleteFile(string path, bool isPersistentPath)
        {
            path = HandlePath(path, isPersistentPath);   
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.LogFormat("[FileManager] File found at {0} successfully deleted", path);
                return true;
            }
            else
            {
                Debug.Log("[FileManager] The file does not exist!");
                return false;
            }
        }
        #endregion

        #region Public helpers
        /// <summary>
        /// Returns a new instance of the class with all properties and fields copied.
        /// </summary>
        public static T CreateCopy<T>(this T source, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public) where T : new() => CopyValues(source, new T(), flags);
        public static T CopyValuesTo<T>(this T source, T target, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public) => CopyValues(source, target, flags);

        /// <summary>
        /// Copies the values of the source to the destination. May not work, if the source is of higher inheritance class than the destination. Does not work with virtual properties.
        /// </summary>
        public static T CopyValues<T>(T source, T destination, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            if (source == null)
            {
                throw new Exception("Failed to copy object. Source is null.");
            }
            if (destination == null)
            {
                throw new Exception("Failed to copy object. Destination is null.");
            }
            Type type = source.GetType();
            var properties = type.GetProperties(flags);
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    property.SetValue(destination, property.GetValue(source, null), null);
                }
            }
            var fields = type.GetFields(flags);
            foreach (var field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
            }
            // Check that the fields match.Uncomment to apply the test, if in doubt.
            //if (fields.Any(f => { var value = f.GetValue(destination); return value == null || !value.Equals(f.GetValue(source)); }))
            //{
            //    throw new Exception("Failed to copy some of the fields.");
            //}
            return destination;
        }

        public struct SeparatedPathPair
        {
            public string pathWithoutFileName;
            public string fullPath;
        }

        public static SeparatedPathPair SeparateFileNameFromPath(string fileName, string path, char separatorCharacter)
        {
            var paths = new SeparatedPathPair();
            if (string.IsNullOrEmpty(path))
            {
                paths.pathWithoutFileName = string.Empty;
                paths.fullPath = separatorCharacter + fileName;
            }
            else
            {
                paths.pathWithoutFileName = path;
                paths.fullPath = path;
                if (path.Contains(fileName) && !path[path.Length - 1].Equals(separatorCharacter))
                {
                    Debug.Log("[FileManager] Filtering the file name from the path");
                    var parsedPath = path.Split(separatorCharacter);
                    var ignorable = parsedPath.Last(p => p == fileName);
                    paths.pathWithoutFileName = string.Join(separatorCharacter.ToString(), parsedPath.Where(p => p != ignorable).ToArray());
                }
                else
                {
                    paths.fullPath = paths.pathWithoutFileName[paths.pathWithoutFileName.Length - 1].Equals(separatorCharacter) ? paths.pathWithoutFileName + fileName : paths.pathWithoutFileName + separatorCharacter + fileName;
                }
            }
            return paths;
        }

        public static string GetRightPartOfPath(string path, string after, char separatorCharacter)
        {
            var parts = path.Split(separatorCharacter);
            int afterIndex = Array.IndexOf(parts, after);
            if (afterIndex < 0)
            {
                throw new Exception(string.Format("[FileManager] Cannot find the right side of path {0} from {1}", path, after));
            }
            return string.Join(separatorCharacter.ToString(), parts, afterIndex, parts.Length - afterIndex);
        }
        #endregion

        #region Private methods
        private static string HandlePath(string path, bool isPersistentPath)
        {
            path = TrimPath(path);
            if (isPersistentPath)
            {
                return Path.Combine(Application.persistentDataPath, path);
            }
            else
            {
                return Application.dataPath + "/" + path;
            }
        }

        private static string TrimPath(string path)
        {
            return path.Replace("//", "/").TrimStart('/', ' ');
        }

        private static SeparatedPathPair ParsePath(string fileName, string path, bool isPersistentPath)
        {
            path = TrimPath(path);
            var paths = SeparateFileNameFromPath(fileName, path, '/');
            if (isPersistentPath)
            {
                paths.fullPath = Path.Combine(Application.persistentDataPath, paths.fullPath);
                paths.pathWithoutFileName = Path.Combine(Application.persistentDataPath, paths.pathWithoutFileName);
            }
            else
            {
                paths.fullPath = Application.dataPath + "/" + paths.fullPath;
                paths.pathWithoutFileName = Application.dataPath + "/" + paths.pathWithoutFileName;
            }
            return paths;
        }
        #endregion
    }
}
