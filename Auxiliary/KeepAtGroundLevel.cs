using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    public class KeepAtGroundLevel : MonoBehaviour
    {
        public string groundLayer;
        private RaycastHit hit;

        public bool smooth;
        public bool useUnscaledTime;
        [SerializeField]
        private float smoothTime = 0.2f;
        private float currentVelocity;
        private float currentY;

        private void Update()
        {
            var mask = groundLayer.NameToMask();
            if (Physics.Raycast(transform.position, Vector3.down, out hit, mask))
            {
                Translate();
            }
            else if (Physics.Raycast(transform.position, Vector3.up, out hit, mask))
            {
                Translate();
            }
        }

        private void Translate()
        {
            var targetPos = transform.position;
            if (smooth)
            {
                currentY = Mathf.SmoothDamp(currentY, hit.point.y, ref currentVelocity, smoothTime, Mathf.Infinity, useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
                targetPos.y = currentY;
            }
            else
            {
                targetPos.y = hit.point.y;
            }
            transform.position = targetPos;
        }
    }
}
