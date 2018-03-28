using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    public class ClampTransformPosition : MonoBehaviour
    {
        public Vector3 min = -UnityHelpers.InfiniteVector;
        public Vector3 max = UnityHelpers.InfiniteVector;

        public enum UpdateMethod
        {
            Update,
            LateUpdate
        }

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
            transform.position = transform.position.Clamp(min, max);
        }
    }
}
