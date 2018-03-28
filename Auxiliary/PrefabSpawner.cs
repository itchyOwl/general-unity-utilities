using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    public class PrefabSpawner : MonoBehaviour
    {
        public enum SpawnMode
        {
            Start,
            OnEnable,
            OnDisable,
            Update,
            FunctionCall
        }

        public enum SpawnType
        {
            OnlyFirst,
            All,
            Random
        }

        public SpawnMode spawnMode;
        public SpawnType spawnType;
        public List<GameObject> prefabs = new List<GameObject>();
        public bool worldPositionStays;
        [Tooltip("Optional")]
        public string instanceName;
        [Tooltip("Optional")]
        public Transform parent;
        [Tooltip("Seconds between spawns. Used only with SpawnMode.Update.")]
        public float threshold;
        [Tooltip("How many times the instance is created per spawn cycle?")]
        public int multiplier = 1;

        /// <summary>
        /// Has the spawner yet spawned anything?
        /// </summary>
        public bool HasSpawned { get; private set; }

        public event Action<GameObject> ObjectSpawned;

        private float timer;

        void Awake()
        {
            if (parent == null) { parent = transform; }
        }

        void Start()
        {
            if (spawnMode == SpawnMode.Start)
            {
                SpawnInternal();
            }
        }

        void OnEnable()
        {
            if (spawnMode == SpawnMode.OnEnable)
            {
                SpawnInternal();
            }
        }

        void OnDisable()
        {
            if (spawnMode == SpawnMode.OnDisable)
            {
                SpawnInternal();
            }
        }

        void Update()
        {
            if (spawnMode == SpawnMode.Update)
            {
                timer += Time.deltaTime;
                if (timer >= threshold)
                {
                    SpawnInternal();
                    timer = 0;
                }
            }
        }

        public void Spawn()
        {
            if (spawnMode == SpawnMode.FunctionCall)
            {
                SpawnInternal();
            }
        }

        private void SpawnInternal()
        {
            HasSpawned = true;
            switch (spawnType)
            {
                case SpawnType.All:
                    for (int i = 0; i < multiplier; i++)
                    {
                        foreach (var p in prefabs)
                        {
                            HandlePrefab(p);
                        }
                    }
                    break;
                case SpawnType.OnlyFirst:
                    var prefab = prefabs.FirstOrDefault();
                    for (int i = 0; i < multiplier; i++)
                    {
                        HandlePrefab(prefab);
                    }
                    break;
                case SpawnType.Random:
                    prefab = prefabs.GetRandom();
                    for (int i = 0; i < multiplier; i++)
                    {
                        HandlePrefab(prefab);
                    }
                    break;
            }
        }

        private GameObject HandlePrefab(GameObject prefab)
        {
            GameObject instance = Instantiate(prefab, parent);
            if (!string.IsNullOrEmpty(instanceName))
            {
                instance.name = instanceName;
            }
            if (ObjectSpawned != null)
            {
                ObjectSpawned(instance);
            }
            return instance;
        }
    }
}
