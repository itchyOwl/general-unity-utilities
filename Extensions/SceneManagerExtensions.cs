using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ItchyOwl.SceneManagement
{
    public static class SceneManagerExtensions
    {
        /// <summary>
        /// Custom event launched before scene unloading.
        /// </summary>
        public static event UnityAction<Scene> BeforeSceneUnload = scene => Debug.Log("[SceneManagerExtensions] Loading scene " + scene.name);

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.LoadScene method.
        /// </summary>
        public static void LoadScene(string sceneName)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.LoadScene method.
        /// </summary>
        public static void LoadScene(string sceneName, LoadSceneMode mode)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            SceneManager.LoadScene(sceneName, mode);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.LoadScene method.
        /// </summary>
        public static void LoadScene(int sceneBuildIndex)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            SceneManager.LoadScene(sceneBuildIndex);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.LoadScene method.
        /// </summary>
        public static void LoadScene(int sceneBuildIndex, LoadSceneMode mode)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.LoadSceneAsync method.
        /// </summary>
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            return SceneManager.LoadSceneAsync(sceneBuildIndex);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.LoadSceneAsync method.
        /// </summary>
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.LoadSceneAsync method.
        /// </summary>
        public static AsyncOperation LoadSceneAsync(string sceneName)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            return SceneManager.LoadSceneAsync(sceneName);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.LoadSceneAsync method.
        /// </summary>
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            return SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.UnloadSceneAsync method.
        /// </summary>
        public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            return SceneManager.UnloadSceneAsync(sceneBuildIndex);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.UnloadSceneAsync method.
        /// </summary>
        public static AsyncOperation UnloadSceneAsync(string sceneName)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            return SceneManager.UnloadSceneAsync(sceneName);
        }

        /// <summary>
        /// Launches BeforeSceneUnload event before calling SceneManager.UnloadSceneAsync method.
        /// </summary>
        public static AsyncOperation UnloadSceneAsync(Scene scene)
        {
            BeforeSceneUnload(SceneManager.GetActiveScene());
            return SceneManager.UnloadSceneAsync(scene);
        }
    }
}
