using UnityEngine;
using System.Collections.Generic;

namespace ItchyOwl.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Returns only the immediate children. Does not include the grandchildren.
        /// </summary>
        public static List<Transform> GetImmediateChildren(this Transform t)
        {
            var children = new List<Transform>();
            for (int i = 0; i < t.childCount; i++)
            {
                children.Add(t.GetChild(i));
            }
            return children;
        }

        /// <summary>
        /// Get screen space distance from world points. For screen space overlay ui, no camera is required. In this case the world point is considered as a screen point so that point 0,0 is at the bottom left corner of the screen.
        /// </summary>
        public static float GetScreenSpaceDistanceTo(this Transform t, Transform other, Camera camera = null)
        {
            return t.GetScreenSpaceDistanceTo(other.position, camera);
        }

        /// <summary>
        /// Get screen space distance from world points. For screen space overlay ui, no camera is required. In this case the world point is considered as a screen point so that point 0,0 is at the bottom left corner of the screen.
        /// </summary>
        public static float GetScreenSpaceDistanceTo(this Transform t, Vector3 worldPoint, Camera camera = null)
        {
            return t.position.GetScreenSpaceDistanceTo(worldPoint, camera);
        }

        public static float GetDistanceTo(this Transform t, Transform target)
        {
            return t.GetDistanceTo(target.position);
        }

        public static float GetDistanceTo(this Transform t, Vector3 targetPos)
        {
            return Vector3.Distance(t.position, targetPos);
        }

        public static float GetAngleTo(this Transform t, Transform target, bool lockY = false)
        {
            return t.GetAngleTo(target.position, lockY);
        }

        public static float GetAngleTo(this Transform t, Vector3 targetPos, bool lockY = false)
        {
            var dir = targetPos - t.position;
            if (lockY)
            {
                dir.y = t.forward.y;
            }
            return Vector3.Angle(t.forward, dir);
        }

        public static Vector3 SelectClosest(this Transform t, Vector3 first, Vector3 second)
        {
            var d1 = first - t.position;
            var d2 = second - t.position;
            if (d1.sqrMagnitude <= d2.sqrMagnitude) { return first; }
            else { return second; }
        }

        public static Vector3 SelectFurthest(this Transform t, Vector3 first, Vector3 second)
        {
            var d1 = first - t.position;
            var d2 = second - t.position;
            if (d1.sqrMagnitude <= d2.sqrMagnitude) { return second; }
            else { return first; }
        }

        /// <summary>
        /// For checking if the distance to target is more or less than x, this method is faster than calculating the actual distance.
        /// </summary>
        public static bool IsCloserToThan(this Transform t, Vector3 targetPos, float distanceLimit)
        {
            Vector3 dir = targetPos - t.position;
            return dir.IsMagnitudeLessThan(distanceLimit);
        }

        /// <summary>
        /// For checking if the distance to target is more or less than x, this method is faster than calculating the actual distance.
        /// </summary>
        public static bool IsFartherFromThan(this Transform t, Vector3 targetPos, float distanceLimit)
        {
            Vector3 dir = targetPos - t.position;
            return dir.IsMagnitudeGreaterThan(distanceLimit);
        }

        public static bool IsTargetBehindMe(this Transform t, Vector3 targetPos)
        {
            var dir = targetPos - t.position;
            var dot = Vector3.Dot(t.forward, dir.normalized);
            return dot < 0;
        }

        public static bool IsTargetOnMyRightSide(this Transform t, Vector3 targetPos)
        {
            var dir = targetPos - t.position;
            var crossProduct = Vector3.Cross(t.forward, dir.normalized);
            return crossProduct.y > 0;
        }

        public static void RotateTowardTarget(this Transform t, Vector3 target, float speed, bool lockYAxis = false, bool inverse = false)
        {
            var dir = target - t.position;
            t.RotateTowardDir(dir, speed, lockYAxis, inverse);
        }

        public static void RotateTowardTarget(this Transform t, Transform target, float speed, bool lockYAxis = false, bool inverse = false)
        {
            var dir = target.position - t.position;
            t.RotateTowardDir(dir, speed, lockYAxis, inverse);
        }

        public static void RotateTowardDir(this Transform t, Vector3 dir, float speed, bool lockYAxis = false, bool inverse = false)
        {
            if (inverse)
            {
                dir = -dir;
            }
            if (lockYAxis)
            {
                dir.y = t.forward.y;
            }
            if (dir != Vector3.zero)
            {
                // TODO: Instead of this misuse of Slerp, consider properly calculating the Slerp (see Tweener) or smoothing the value with SmoothStep.
                t.rotation = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(dir), Time.deltaTime * speed);
            }
        }

        /// <summary>
        /// Max angle is 180.
        /// </summary>
        public static bool IsFacingDir(this Transform t, Vector3 dir, float angle = 0, bool lockYAxis = false, bool inverse = false)
        {
            if (inverse)
            {
                dir = -dir;
            }
            if (lockYAxis)
            {
                dir.y = t.forward.y;
            }
            return t.forward.IsFacing(dir, angle);
        }

        /// <summary>
        /// Max angle is 180.
        /// </summary>
        public static bool IsFacingPos(this Transform t, Vector3 target, float angle = 0, bool lockYAxis = false, bool inverse = false)
        {
            var dir = target - t.position;
            return t.IsFacingDir(dir, angle, lockYAxis, inverse);
        }

        /// <summary>
        /// Max angle is 180.
        /// </summary>
        public static bool IsFacing(this Transform t, Transform target, float angle = 0, bool lockYAxis = false, bool inverse = false)
        {
            return IsFacingPos(t, target.position, angle, lockYAxis, inverse);
        }

        public static Vector3 TranslatePointTowardSelf(this Transform t, Vector3 pos, float distance)
        {
            return pos.TranslatePointTowardPoint(t.position, distance);
        }
    }
}

