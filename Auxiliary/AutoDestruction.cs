using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public class AutoDestruction : MonoBehaviour
    {
        [Tooltip("Automatically initializes destruction each time OnEnable is called.")]
        public bool autoInitializeDestruction;
        [Tooltip("Changing the value after the destruction has been initialized, does not have an effect. However, recalling InitializeDestruction resets the routine properly.")]
        public float lifeTime = Mathf.Infinity;
        public bool IsDestructionInitialized { get; private set; }

        private float timer;

        private void OnEnable()
        {
            if (autoInitializeDestruction)
            {
                InitializeDestruction(lifeTime);
            }
        }

        public void InitializeDestruction(float time)
        {
            IsDestructionInitialized = true;
            lifeTime = time;
        }

        private void Update()
        {
            if (!IsDestructionInitialized) { return; }
            timer += Time.deltaTime;
            if (timer >= lifeTime)
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
