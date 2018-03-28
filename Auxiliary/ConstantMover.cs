using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public class ConstantMover : MonoBehaviour
    {
        public Vector3 translationDir;

        void Update()
        {
            transform.position += translationDir * Time.deltaTime;
        }
    }
}

