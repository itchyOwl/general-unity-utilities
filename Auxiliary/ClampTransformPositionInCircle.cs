using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    public class ClampTransformPositionInCircle : MonoBehaviour
    {
        public Transform centerTarget;
        public Vector2 centerPoint;
        public float radius;

        public enum UpdateMethod
        {
            Update,
            LateUpdate
        }

        public VectorExtensions.Axis axis = VectorExtensions.Axis.XZ;
        public UpdateMethod updateMethod;

        private void Update()
        {
            if (updateMethod != UpdateMethod.Update) { return; }
            Clamp();
        }

        private void LateUpdate()
        {
            if (updateMethod != UpdateMethod.LateUpdate) { return; }
            Clamp();
        }

        private void Clamp()
        {
            if (centerTarget != null)
            {
                centerPoint = centerTarget.position.TransformVector(axis);
            }
            var currentPos2d = transform.position.TransformVector(axis);
            var fromCenter = currentPos2d - centerPoint;
            var newPos2d = centerPoint + Vector2.ClampMagnitude(fromCenter, radius);
            transform.position = newPos2d.TransformVector(axis, transform.position.y);
        }
    }
}
