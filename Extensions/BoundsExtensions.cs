using UnityEngine;
using System.Collections;

namespace ItchyOwl.Extensions
{
    public static class BoundsExtensions
    {
        /// <summary>
        /// Calculates a screen space rectangle from world space bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static Rect ToScreenSpace(this Bounds bounds, Camera camera)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            Vector2[] cornerPoints = new Vector2[8]
            {
                camera.WorldToScreenPoint(bounds.min),
                camera.WorldToScreenPoint(new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z)),
                camera.WorldToScreenPoint(new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z)),
                camera.WorldToScreenPoint(new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z)),
                camera.WorldToScreenPoint(new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z)),
                camera.WorldToScreenPoint(new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z)),
                camera.WorldToScreenPoint(new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z)),
                camera.WorldToScreenPoint(bounds.max)
            };
            Vector2 min = cornerPoints[0];
            Vector2 max = cornerPoints[7];
            foreach (var corner in cornerPoints)
            {
                min = Vector2.Min(min, corner);
                max = Vector2.Max(max, corner);
            }
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }
    }
}
