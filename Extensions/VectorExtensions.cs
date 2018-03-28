using UnityEngine;
using ItchyOwl.General;
using System;

namespace ItchyOwl.Extensions
{
    public static class VectorExtensions
    {
        public enum Axis
        {
            XZ,
            XY,
        }

        public static Vector2 TransformVector(this Vector3 vector, Axis axis)
        {
            switch (axis)
            {
                case Axis.XZ:
                    return new Vector2(vector.x, vector.z);
                case Axis.XY:
                    return new Vector2(vector.x, vector.y);
                default:
                    throw new NotImplementedException(axis.ToString());
            }
        }

        public static Vector3 TransformVector(this Vector2 vector, Axis axis, float thirdAxis)
        {
            switch (axis)
            {
                case Axis.XZ:
                    return new Vector3(vector.x, thirdAxis, vector.y);
                case Axis.XY:
                    return new Vector3(vector.x, vector.y, thirdAxis);
                default:
                    throw new NotImplementedException(axis.ToString());
            }
        }

        public static Vector3 TranslatePointTowardPoint(this Vector3 originalPoint, Vector3 targetPoint, float distance)
        {
            var toPos = targetPoint - originalPoint;
            var translation = toPos.normalized * distance;
            return originalPoint + translation;
        }

        public static Vector2 TranslatePointTowardPoint(this Vector2 originalPoint, Vector2 targetPoint, float distance)
        {
            var toPos = targetPoint - originalPoint;
            var translation = toPos.normalized * distance;
            return originalPoint + translation;
        }

        /// <summary>
        /// Get screen space distance from world points. For screen space overlay ui, no camera is required. In this case the world point is considered as a screen point so that point 0,0 is at the bottom left corner of the screen.
        /// </summary>
        public static float GetScreenSpaceDistanceTo(this Vector3 v, Vector3 worldPoint, Camera camera = null)
        {
            Vector2 p1 = RectTransformUtility.WorldToScreenPoint(camera, v);
            Vector2 p2 = RectTransformUtility.WorldToScreenPoint(camera, worldPoint);
            return Vector2.Distance(p1, p2);
        }

        public static float[] ToArray(this Vector2 v)
        {
            return new float[2] { v.x, v.y };
        }

        public static float[] ToArray(this Vector3 v)
        {
            return new float[3] { v.x, v.y, v.z };
        }

        public static float[] ToArray(this Vector4 v)
        {
            return new float[4] { v.x, v.y, v.z, v.w };
        }

        public static SerializableStructs.Vector2 AsSerializable(this Vector2 v)
        {
            return new SerializableStructs.Vector2(v.x, v.y);
        }

        public static SerializableStructs.Vector3 AsSerializable(this Vector3 v)
        {
            return new SerializableStructs.Vector3(v.x, v.y, v.z);
        }

        public static SerializableStructs.Vector4 AsSerializable(this Vector4 v)
        {
            return new SerializableStructs.Vector4(v.x, v.y, v.z, v.w);
        }

        #region Math
        public static Vector2 Subtract(this Vector2 v, Vector2 vector)
        {
            return v - vector;
        }

        public static Vector2 Add(this Vector2 v, Vector2 vector)
        {
            return v + vector;
        }

        public static Vector3 Subtract(this Vector3 v, Vector3 vector)
        {
            return v - vector;
        }

        public static Vector3 Add(this Vector3 v, Vector3 vector)
        {
            return v + vector;
        }

        public static bool IsMagnitudeGreaterThan(this Vector3 v, float f)
        {
            return v.sqrMagnitude > f * f;
        }

        public static bool IsMagnitudeGreaterOrEqualTo(this Vector3 v, float f)
        {
            return v.sqrMagnitude >= f * f;
        }

        public static bool IsMagnitudeLessOrEqualTo(this Vector3 v, float f)
        {
            return v.sqrMagnitude <= f * f;
        }

        public static bool IsMagnitudeLessThan(this Vector3 v, float f)
        {
            return v.sqrMagnitude < f * f;
        }

        public static bool IsMagnitudeGreaterThan(this Vector2 v, float f)
        {
            return v.sqrMagnitude > f * f;
        }

        public static bool IsMagnitudeGreaterOrEqualTo(this Vector2 v, float f)
        {
            return v.sqrMagnitude >= f * f;
        }

        public static bool IsMagnitudeLessOrEqualTo(this Vector2 v, float f)
        {
            return v.sqrMagnitude <= f * f;
        }

        public static bool IsMagnitudeLessThan(this Vector2 v, float f)
        {
            return v.sqrMagnitude < f * f;
        }

        public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
        {
            float x = Mathf.Clamp(vector.x, min.x, max.x);
            float y = Mathf.Clamp(vector.y, min.y, max.y);
            float z = Mathf.Clamp(vector.z, min.z, max.z);
            return new Vector3(x, y, z);
        }

        public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
        {
            float x = Mathf.Clamp(vector.x, min.x, max.x);
            float y = Mathf.Clamp(vector.y, min.y, max.y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Compares the values individually (not the magnitudes).
        /// TODO: Test! This function might not work as intended!
        /// </summary>
        public static bool IsCloseTo(this Vector2 v, Vector2 other, float margin)
        {
            if (v.Equals(other))
            {
                return true;
            }
            else
            {
                return Mathf.Abs(v.x - other.x) < margin && Mathf.Abs(v.y - other.y) < margin;
            }
        }

        public static bool IsFacing(this Vector3 forward, Vector3 dir, float angle = 0)
        {
            return Vector3.Angle(forward, dir) <= angle;
        }

        public static bool IsFacing(this Vector2 forward, Vector3 dir, float angle = 0)
        {
            return Vector2.Angle(forward, dir) <= angle;
        }
        #endregion
    }
}
