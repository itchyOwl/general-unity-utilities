using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using ItchyOwl.UI;

namespace ItchyOwl.Auxiliary
{
    public class WebLoader : MonoBehaviour
    {
        protected Coroutine webLoadCoroutine;

        /// <summary>
        /// Attempts 0 (default) is infinite.
        /// </summary>
        public void StartWebLoad(string url, bool allowOnlyOne = false, int attempts = 0, float secondsBetweenAttempts = 5, Action<UnityWebRequest> callbackIfSuccessful = null, Action callbackIfFails = null)
        {
            if (allowOnlyOne && webLoadCoroutine != null)
            {
                Debug.LogWarning("[WebLoader]: Only one coroutine allowed!");
            }
            webLoadCoroutine = StartCoroutine(WebLoad(url, attempts, secondsBetweenAttempts, callbackIfSuccessful, callbackIfFails));
        }

        protected IEnumerator WebLoad(string url, int attempts = 0, float secondsBetweenAttempts = 5, Action<UnityWebRequest> callbackIfSuccessful = null, Action callbackIfFails = null)
        {
            var request = new UnityWebRequest(url);
            yield return request;
            if (request.error != null)
            {
                Debug.LogWarning("[WebLoader] " + request.error);
                if (attempts == 0)
                {
                    while (request.error != null)
                    {
                        GUIManager.CreateNotification(string.Format("HTTP error, trying again in {0} seconds...", secondsBetweenAttempts));
                        yield return new WaitForSeconds(secondsBetweenAttempts);
                        request = new UnityWebRequest(url);
                        yield return request;
                    }
                }
                else
                {
                    for (int i = 0; i < attempts; i++)
                    {
                        GUIManager.CreateNotification(string.Format("HTTP error, trying again in {0} seconds...", secondsBetweenAttempts));
                        yield return new WaitForSeconds(secondsBetweenAttempts);
                        request = new UnityWebRequest(url);
                        yield return request;
                    }
                }
            }
            if (request.error == null)
            {
                if (callbackIfSuccessful != null)
                {
                    callbackIfSuccessful(request);
                }
            }
            else if (callbackIfFails != null)
            {
                callbackIfFails();
            }
            webLoadCoroutine = null;
        }
    }
}


