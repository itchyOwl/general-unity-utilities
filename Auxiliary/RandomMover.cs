using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public class RandomMover : MonoBehaviour
    {
        public float speed = 1;

        private void Update()
        {
            transform.position = transform.position + UnityHelpers.RandomVector3(-1, 1) * speed * Time.deltaTime;
        }
    }
}

