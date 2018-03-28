using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ItchyOwl.Extensions;
using System;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Handles setting a material and resetting the original material on multiple renderers with one or more materials.
    /// Filters renderers with predefined criteria.
    /// </summary>
    public class MaterialManager : MonoBehaviour
    {
        public bool useSharedMaterials;     // Don't use if the materials are manipulated instead of simple toggled on and off.
        public bool includeInactive = true;
        public bool filterRenderers;
        public bool setRenderQueue;
        public int renderQueue = 3000;

        // Optional filters
        public List<string> skipRendererNames = new List<string>();
        public List<System.Type> skipRendererTypes = new List<System.Type>();

        private Dictionary<Renderer, Material[]> _renderers;
        public Dictionary<Renderer, Material[]> Renderers
        {
            get
            {
                if (_renderers == null)
                {
                    if (filterRenderers)
                    {
                        FilterRenderers();
                    }
                    else
                    {
                        _renderers = GetComponentsInChildren<Renderer>(includeInactive: includeInactive).ToDictionary(r => r, r => r.sharedMaterials);
                    }
                }
                return _renderers;
            }
        }

        public void FilterRenderers()
        {
            filterRenderers = true;
            var all = GetComponentsInChildren<Renderer>(includeInactive: includeInactive);
            var filteredByType = all.Where(r => skipRendererTypes.Contains(r.GetType()));
            var filteredByName = all.Where(r => skipRendererNames.Any(n => r.name.ToLower().Contains(n)));    // Lower case letters enforced to make the comparison easier.
            var accepted = all.Where(r => !filteredByName.Contains(r) && !filteredByType.Contains(r));
            //filteredByType.ForEach(r => Debug.Log("[MaterialManager] Renderer filtered by type: " + r));
            //filteredByName.ForEach(r => Debug.Log("[MaterialManager] Renderer filtered by name: " + r));
            //accepted.ForEach(r => Debug.Log("[MaterialManager] Renderer accepted: " + r));
            _renderers = accepted.ToDictionary(r => r, r => r.sharedMaterials);
        }

        public void SetMaterial(Material material)
        {
            Renderers.ForEach(r => SetMaterials(r.Key, material));
        }

        public void UpdateMaterial(Action<Material> updateCallback)
        {
            Renderers.ForEach(r => UpdateMaterials(r.Key, updateCallback));
        }

        public void RestoreOriginalMaterials()
        {
            foreach (var r in Renderers)
            {
                var renderer = r.Key;
                var originalMaterials = r.Value;
                if (useSharedMaterials)
                {
                    renderer.sharedMaterials = originalMaterials;
                }
                else
                {
                    renderer.materials = originalMaterials;
                }
            }
        }

        private void SetMaterials(Renderer renderer, Material mat)
        {
            if (setRenderQueue)
            {
                mat.renderQueue = renderQueue;
            }
            if (useSharedMaterials)
            {
                if (renderer.sharedMaterials.Length > 1)
                {
                    var materials = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = mat;
                    }
                    renderer.sharedMaterials = materials;
                }
                else
                {
                    renderer.sharedMaterial = mat;
                }
            }
            else
            {
                if (renderer.materials.Length > 1)
                {
                    var materials = new Material[renderer.materials.Length];
                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = mat;
                    }
                    renderer.materials = materials;
                }
                else
                {
                    renderer.material = mat;
                }
            }
        }

        private void UpdateMaterials(Renderer renderer, Action<Material> updateCallback)
        {
            if (useSharedMaterials)
            {
                if (renderer.sharedMaterials.Length > 1)
                {
                    var materials = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < materials.Length; i++)
                    {
                        var newMat = renderer.sharedMaterials[i];
                        updateCallback(newMat);
                        if (setRenderQueue)
                        {
                            newMat.renderQueue = renderQueue;
                        }
                        materials[i] = newMat;
                    }
                    renderer.sharedMaterials = materials;
                }
                else
                {
                    var newMat = renderer.sharedMaterial;
                    if (setRenderQueue)
                    {
                        newMat.renderQueue = renderQueue;
                    }
                    updateCallback(newMat);
                    renderer.sharedMaterial = newMat;
                }
            }
            else
            {
                if (renderer.materials.Length > 1)
                {
                    var materials = new Material[renderer.materials.Length];
                    for (int i = 0; i < materials.Length; i++)
                    {
                        var newMat = renderer.materials[i];
                        if (setRenderQueue)
                        {
                            newMat.renderQueue = renderQueue;
                        }
                        updateCallback(newMat);
                        materials[i] = newMat;
                    }
                    renderer.materials = materials;
                }
                else
                {
                    var newMat = renderer.material;
                    if (setRenderQueue)
                    {
                        newMat.renderQueue = renderQueue;
                    }
                    updateCallback(newMat);
                    renderer.material = newMat;
                }
            }
        }
    }
}
