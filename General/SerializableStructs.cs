using System;

namespace ItchyOwl.General
{
    /// <summary>
    /// Unity structs are not marked as serializable and cannot thus be serialized by .Net BinaryFormatter. 
    /// One solution around this, is to convert the Unity structs to custom structs before saving them in binary format. 
    /// Note that this is required only for binary.
    /// </summary>
    public class SerializableStructs
    {
        [Serializable]
        public struct Vector2
        {
            public readonly float x, y;

            public Vector2(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public UnityEngine.Vector2 ToUnityVector()
            {
                return new UnityEngine.Vector2(x, y);
            }
        }

        [Serializable]
        public struct Vector3
        {
            public readonly float x, y, z;

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public UnityEngine.Vector3 ToUnityVector()
            {
                return new UnityEngine.Vector3(x, y, z);
            }
        }

        [Serializable]
        public struct Vector4
        {
            public readonly float x, y, z, w;

            public Vector4(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            public UnityEngine.Vector4 ToUnityVector()
            {
                return new UnityEngine.Vector4(x, y, z, w);
            }
        }

        [Serializable]
        public readonly struct Color
        {
            public float r, g, b, a;
            
            public Color(float r, float g, float b, float a)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }

            public UnityEngine.Color ToUnityColor()
            {
                return new UnityEngine.Color(r, g, b, a);
            }
        }
    }
}
