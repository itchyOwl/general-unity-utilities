using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ItchyOwl.SceneManagement;

namespace ItchyOwl.Auxiliary
{
    public class SceneLoader : MonoBehaviour
    {
        public string sceneName;
        public LoadSceneMode loadMode;
        public bool async;
        public bool destroySelfWhenDone;
        public bool destroyObjectsBeforeLoading;
        public List<GameObject> objectsToDestroy = new List<GameObject>();

        private bool isLoading;

        public bool loadOnStart;

        public void Start()
        {
            if (loadOnStart)
            {
                Load();
            }
        }

        public void Load()
        {
            if (isLoading)
            {
                Debug.LogWarning("[SceneLoader] Already loading");
                return;
            }
            if (!async)
            {
                if (destroyObjectsBeforeLoading)
                {
                    Destroy(objectsToDestroy);
                }
                SceneManagerExtensions.LoadScene(sceneName, loadMode);
                if (destroySelfWhenDone) { Destroy(gameObject); }
            }
            else
            {
                StartCoroutine(LoadSceneAsync());
            }
        }

        private void Destroy(List<GameObject> objectsToDestroy)
        {
            int count = objectsToDestroy.Count;
            for (int i = 0; i < count; i++)
            {
                Destroy(objectsToDestroy[i]);
            }
        }

        private IEnumerator LoadSceneAsync()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[SceneLoade] Define the level name");
                yield break;
            }
            isLoading = true;
            if (destroyObjectsBeforeLoading)
            {
                Destroy(objectsToDestroy);
            }
            yield return SceneManagerExtensions.LoadSceneAsync(sceneName, loadMode);
            if (destroySelfWhenDone) { Destroy(gameObject); }
            isLoading = false;
        }
    }
}

