using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public class SetRandomPositionInSphere : MonoBehaviour
    {
        public Vector3 center;
        public float radius;
        public SetupMode mode;

        public enum SetupMode
        {
            Awake,
            Start,
            OnEnable
        }

        private void Awake()
        {
            if (mode == SetupMode.Awake)
            {
                SetPosition();
            }
        }

        private void Start()
        {
            if (mode == SetupMode.Start)
            {
                SetPosition();
            }
        }

        private void OnEnable()
        {
            if (mode == SetupMode.OnEnable)
            {
                SetPosition();
            }
        }

        private void SetPosition()
        {
            transform.position = center + radius * Random.insideUnitSphere;
        }
    }
}

