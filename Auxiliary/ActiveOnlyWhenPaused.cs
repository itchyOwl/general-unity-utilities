using UnityEngine;
using System;

namespace ItchyOwl.Auxiliary
{
    public class ActiveOnlyWhenPaused : MonoBehaviour
    {
        private void Start()
        {
            PauseController.GamePaused += OnGamePaused;
            PauseController.GameContinue += OnGameContinue;
            gameObject.SetActive(PauseController.IsPaused);
        }

        private void OnDestroy()
        {
            PauseController.GamePaused -= OnGamePaused;
            PauseController.GameContinue -= OnGameContinue;
        }

        private void OnGamePaused(object sender, EventArgs args)
        {
            gameObject.SetActive(true);
        }

        private void OnGameContinue(object sender, EventArgs args)
        {
            gameObject.SetActive(false);
        }
    }
}
