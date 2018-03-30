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

            public static implicit operator UnityEngine.Vector2(Vector2 v)
            {
                return new UnityEngine.Vector2(v.x, v.y);
            }

            public static implicit operator Vector2(UnityEngine.Vector2 v)
            {
                return new Vector2(v.x, v.y);
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

            public static implicit operator UnityEngine.Vector3(Vector3 v)
            {
                return new UnityEngine.Vector3(v.x, v.y, v.z);
            }

            public static implicit operator Vector3(UnityEngine.Vector3 v)
            {
                return new Vector3(v.x, v.y, v.z);
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

            public static implicit operator UnityEngine.Vector3(Vector4 v)
            {
                return new UnityEngine.Vector4(v.x, v.y, v.z);
            }

            public static implicit operator Vector4(UnityEngine.Vector4 v)
            {
                return new Vector4(v.x, v.y, v.z, v.w);
            }
        }

        [Serializable]
        public struct Color
        {
            public readonly float r, g, b, a;
            
            public Color(float r, float g, float b, float a)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }

            public static implicit operator UnityEngine.Color(Color c)
            {
                return new UnityEngine.Color(c.r, c.g, c.b, c.a);
            }

            public static implicit operator Color(UnityEngine.Color c)
            {
                return new Color(c.r, c.g, c.b, c.a);
            }
        }
    }
}
