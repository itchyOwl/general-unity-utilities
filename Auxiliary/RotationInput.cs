using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public class RotationInput : MonoBehaviour
    {
        public float speed = 50;
        /// <summary>
        /// Defined in the Unity InputManager.
        /// </summary>
        public string rotationAxisName = "CameraRotation";
        public Space space = Space.Self;
        public bool useUnscaledTime;
        [SerializeField]
        private float axisSmoothTime = 0.1f;    // Used in smoothening the raw axis input only when unscaledTime is used.
        public enum Axis { X, Y, Z }
        public Axis axis = Axis.Y;

        public enum UpdateMode
        {
            Update,
            LateUpdate
        }

        public UpdateMode updateMode = UpdateMode.Update;

        private void Update()
        {
            if (updateMode != UpdateMode.Update) { return; }
            Rotate();
        }

        void LateUpdate()
        {
            if (updateMode != UpdateMode.LateUpdate) { return; }
            Rotate();
        }

        private void Rotate()
        {
            Vector3 rotation;
            switch (axis)
            {
                case Axis.X:
                    rotation = new Vector3(CalculateInputAxis(), 0, 0);
                    break;
                case Axis.Y:
                    rotation = new Vector3(0, CalculateInputAxis(), 0);
                    break;
                case Axis.Z:
                    rotation = new Vector3(0, 0, CalculateInputAxis());
                    break;
                default:
                    rotation = Vector3.zero;
                    break;
            }
            switch (space)
            {
                case Space.Self:
                    transform.localEulerAngles += rotation;
                    break;
                case Space.World:
                    transform.eulerAngles += rotation;
                    break;
            }
        }

        private float CalculateInputAxis()
        {
            if (useUnscaledTime)
            {
                return InputHelpers.GetAxisRawSmooth(rotationAxisName, Time.unscaledDeltaTime, axisSmoothTime) * speed * Time.unscaledDeltaTime;
            }
            else
            {
                return Input.GetAxis(rotationAxisName) * speed * Time.deltaTime;
            }
        }
    }
}
