using System.Collections.Generic;
using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public class ToggleGameObjects : MonoBehaviour
    {
        public List<GameObject> gameObjects = new List<GameObject>();

        public void Toggle()
        {
            gameObjects.ForEach(go => go.SetActive(!go.activeInHierarchy));
        }
    }
}

