using System;
using System.Runtime.Serialization;
using UnityEngine;

/// <summary>
/// Modified from: https://forum.unity3d.com/threads/unity-is-a-bad-joke.393688/
/// References: 
/// https://msdn.microsoft.com/en-us/library/system.runtime.serialization.surrogateselector(v=vs.110).aspx
/// https://msdn.microsoft.com/en-us/library/system.runtime.serialization.isurrogateselector(v=vs.110).aspx
/// Note: This throws a serialization exception, which could propably be avoided by using a custom serializable class.
/// </summary>
namespace ItchyOwl.DataManagement
{
    /// <summary>
    /// Enables serialization of the following data types: Vector2, Vector3, Vector4, Quaternion, Color, LayerMask, and Matrix4x4.
    /// </summary>
    public class UnityStructSurrogate : ISerializationSurrogate, ISurrogateSelector
    {
        private ISurrogateSelector _nextSelector;

        #region ISerializationSurrogate Interface
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is Vector2)
            {
                AddValue(info, (Vector2)obj);
            }
            else if (obj is Vector3)
            {
                AddValue(info, (Vector3)obj);
            }
            else if (obj is Vector4)
            {
                AddValue(info, (Vector4)obj);
            }
            else if (obj is Quaternion)
            {
                AddValue(info, (Quaternion)obj);
            }
            else if (obj is Color)
            {
                AddValue(info, (Color)obj);
            }
            else if (obj is LayerMask)
            {
                AddValue(info, (LayerMask)obj);
            }
            else if (obj is Matrix4x4)
            {
                AddValue(info, (Matrix4x4)obj);
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (obj is Vector2)
            {
                return GetVector2(info);
            }
            else if (obj is Vector3)
            {
                return GetVector3(info);
            }
            else if (obj is Vector4)
            {
                return GetVector4(info);
            }
            else if (obj is Quaternion)
            {
                return GetQuaternion(info);
            }
            else if (obj is Color)
            {
                return GetColor(info);
            }
            else if (obj is LayerMask)
            {
                return GetLayerMask(info);
            }
            else if (obj is Matrix4x4)
            {
                return GetMatrix4x4(info);
            }
            return null;
        }
        #endregion

        #region Static adders
        public static void AddValue(SerializationInfo info, Vector2 value)
        {
            if (info == null)
            {
                throw new ArgumentNullException("SerializationInfo is null.");
            }
            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
        }

        public static void AddValue(SerializationInfo info, Vector3 value)
        {
            if (info == null)
            {
                throw new ArgumentNullException("SerializationInfo is null.");
            }
            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("z", value.z);
        }

        public static void AddValue(SerializationInfo info, Vector4 value)
        {
            if (info == null)
            {
                throw new ArgumentNullException("SerializationInfo is null.");
            }
            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("z", value.z);
            info.AddValue("w", value.w);
        }

        public static void AddValue(SerializationInfo info, Quaternion value)
        {
            if (info == null)
            {
                throw new ArgumentNullException("SerializationInfo is null.");
            }
            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("z", value.z);
            info.AddValue("w", value.w);
        }

        public static void AddValue(SerializationInfo info, Color value)
        {
            if (info == null)
            {
                throw new ArgumentNullException("SerializationInfo is null.");
            }
            info.AddValue("r", value.r);
            info.AddValue("g", value.g);
            info.AddValue("b", value.b);
            info.AddValue("a", value.a);
        }

        public static void AddValue(SerializationInfo info, LayerMask value)
        {
            if (info == null)
            {
                throw new ArgumentNullException("SerializationInfo is null.");
            }
            info.AddValue("mask", value.value);
        }

        public static void AddValue(SerializationInfo info, Matrix4x4 value)
        {
            if (info == null)
            {
                throw new ArgumentNullException("SerializationInfo is null.");
            }
            info.AddValue("m00", value.m00);
            info.AddValue("m01", value.m01);
            info.AddValue("m02", value.m02);
            info.AddValue("m03", value.m03);
            info.AddValue("m10", value.m10);
            info.AddValue("m11", value.m11);
            info.AddValue("m12", value.m12);
            info.AddValue("m13", value.m13);
            info.AddValue("m20", value.m20);
            info.AddValue("m21", value.m21);
            info.AddValue("m22", value.m22);
            info.AddValue("m23", value.m23);
            info.AddValue("m30", value.m30);
            info.AddValue("m31", value.m31);
            info.AddValue("m32", value.m32);
            info.AddValue("m33", value.m33);
        }
        #endregion

        #region Static getters
        public static Vector2 GetVector2(SerializationInfo info)
        {
            return new Vector2(info.GetSingle("x"), info.GetSingle("y"));
        }

        public static Vector3 GetVector3(SerializationInfo info)
        {
            return new Vector3(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"));
        }

        public static Vector4 GetVector4(SerializationInfo info)
        {
            return new Vector4(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"), info.GetSingle("w"));
        }

        public static Quaternion GetQuaternion(SerializationInfo info)
        {
            return new Quaternion(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"), info.GetSingle("w"));
        }

        public static Color GetColor(SerializationInfo info)
        {
            return new Color(info.GetSingle("r"), info.GetSingle("g"), info.GetSingle("b"), info.GetSingle("a"));
        }

        public static LayerMask GetLayerMask(SerializationInfo info)
        {
            var m = new LayerMask();
            m.value = info.GetInt32("mask");
            return m;
        }

        public static Matrix4x4 GetMatrix4x4(SerializationInfo info)
        {
            var m = new Matrix4x4();
            m.m00 = info.GetSingle("m00");
            m.m01 = info.GetSingle("m01");
            m.m02 = info.GetSingle("m02");
            m.m03 = info.GetSingle("m03");
            m.m10 = info.GetSingle("m10");
            m.m11 = info.GetSingle("m11");
            m.m12 = info.GetSingle("m12");
            m.m13 = info.GetSingle("m13");
            m.m20 = info.GetSingle("m20");
            m.m21 = info.GetSingle("m21");
            m.m22 = info.GetSingle("m22");
            m.m23 = info.GetSingle("m23");
            m.m30 = info.GetSingle("m30");
            m.m31 = info.GetSingle("m31");
            m.m32 = info.GetSingle("m32");
            m.m33 = info.GetSingle("m33");
            return m;
        }
        #endregion

        #region ISurrogateSelector Interface
        public void ChainSelector(ISurrogateSelector selector)
        {
            if (selector == this)
            {
                throw new ArgumentException("Cyclical SurrogateSelector");
            }
            if (_nextSelector == null)
            {
                _nextSelector = selector;
            }
            else
            {
                _nextSelector.ChainSelector(selector);
            }
        }

        public ISurrogateSelector GetNextSelector()
        {
            return _nextSelector;
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (IsSpeciallySerialized(type))
            {
                selector = this;
                return this;
            }
            else if (_nextSelector != null)
            {
                return _nextSelector.GetSurrogate(type, context, out selector);
            }
            else
            {
                selector = null;
                return null;
            }
        }
        #endregion

        #region Is Serialized Test
        private static Type[] _specialTypes = new Type[]
        {
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Matrix4x4),
            typeof(Color),
            typeof(LayerMask)
        };

        public static bool IsSpeciallySerialized(Type tp)
        {
            if (tp == null)
            {
                throw new ArgumentNullException("Type null");
            }
            return Array.IndexOf(_specialTypes, tp) >= 0;
        }
        #endregion
    }
}
