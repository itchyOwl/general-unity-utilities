using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public class TranslationInput : MonoBehaviour
    {
        public float speed = 25;

        /// <summary>
        /// Defined in the Unity InputManager.
        /// </summary>
        public string verticalAxisName = "Vertical";
        /// <summary>
        /// Defined in the Unity InputManager.
        /// </summary>
        public string horizontalAxisName = "Horizontal";

        public UpdateMode updateMode = UpdateMode.Update;
        public TranslationMode mode = TranslationMode.XZ;
        public Space space = Space.Self;
        public bool useUnscaledTime;
        [SerializeField]
        private float axisSmoothTime = 0.1f;    // Used in smoothening the raw axis input only when unscaledTime is used.

        public enum UpdateMode
        {
            Update,
            LateUpdate
        }

        public enum TranslationMode
        {
            XY,
            XZ
        }

        private void Update()
        {
            if (updateMode != UpdateMode.Update) { return; }
            Translate();
        }

        void LateUpdate()
        {
            if (updateMode != UpdateMode.LateUpdate) { return; }
            Translate();
        }

        private void Translate()
        {
            var right = space == Space.Self ? transform.right : Vector3.right;
            right *= GetInput(horizontalAxisName);
            switch (mode)
            {
                case TranslationMode.XY:
                    var up = space == Space.Self ? transform.up : Vector3.up;
                    up *= GetInput(verticalAxisName);
                    var translation = up + right;
                    transform.position += translation;
                    break;
                case TranslationMode.XZ:
                    var forward = space == Space.Self ? transform.forward : Vector3.forward;
                    forward *= GetInput(verticalAxisName);
                    translation = forward + right;
                    transform.position += translation;
                    break;
            }
        }

        private float GetInput(string axisName)
        {
            if (useUnscaledTime)
            {
                return InputHelpers.GetAxisRawSmooth(axisName, Time.unscaledDeltaTime, axisSmoothTime) * speed * Time.unscaledDeltaTime;
            }
            else
            {
                return Input.GetAxis(axisName) * speed * Time.deltaTime;
            }
        }
    }
}
