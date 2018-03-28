using System;
using UnityEngine;

namespace ItchyOwl.Extensions
{
    public static class MaterialExtensions
    {
        /// <summary>
        /// Note: Does not automatically rebuild the textures.
        /// </summary>
        public static void SetProceduralFloatProperty(this Material material, string propertyName, float propertyValue, bool suspendWarnings = false)
        {
            material.SetProceduralProperty(propertyName, s => s.SetProceduralFloat(propertyName, propertyValue), suspendWarnings);
        }

        /// <summary>
        /// Note: Does not automatically rebuild the textures.
        /// </summary>
        public static void SetProceduralColorProperty(this Material material, string propertyName, Color propertyValue, bool suspendWarnings = false)
        {
            material.SetProceduralProperty(propertyName, s => s.SetProceduralColor(propertyName, propertyValue), suspendWarnings);
        }

        /// <summary>
        /// Note: Does not automatically rebuild the textures.
        /// </summary>
        public static void SetProceduralMaterialSeed(this Material material, int seed, bool suspendWarnings = false)
        {
            material.SetProceduralFloatProperty("$randomseed", seed, suspendWarnings);
        }

        /// <summary>
        /// Note: Does not automatically rebuild the textures.
        /// </summary>
        public static void SetProceduralMaterialRandomSeed(this Material material)
        {
            material.SetProceduralMaterialSeed(UnityEngine.Random.Range(0, int.MaxValue));
        }

        /// <summary>
        /// Note: Does not automatically rebuild the textures.
        /// </summary>
        public static void SetProceduralProperty(this Material material, string propertyName, Action<ProceduralMaterial> updateAction, bool suspendWarnings = false)
        {
            var substance = material as ProceduralMaterial;
            if (substance != null)
            {
                if (substance.HasProceduralProperty(propertyName))
                {
                    updateAction(substance);
                }
                else if (!suspendWarnings)
                {
                    Debug.LogWarningFormat("[MaterialExtensions] Property {0} not found from {1}.", propertyName, material.name);
                }
            }
            else if (!suspendWarnings)
            {
                Debug.LogWarningFormat("[MaterialExtensions] Material {0} is not of type ProceduralMaterial.", material.name);
            }
        }

        public static void RebuildProceduralTexturesAsync(this Material material)
        {
            var substance = material as ProceduralMaterial;
            if (substance != null)
            {
                substance.RebuildTextures();
            }
        }

        public static void RebuildProceduralTexturesImmediate(this Material material)
        {
            var substance = material as ProceduralMaterial;
            if (substance != null)
            {
                substance.RebuildTexturesImmediately();
            }
        }

        public static bool IsProcessingProceduralTextures(this Material material)
        {
            var substance = material as ProceduralMaterial;
            if (substance == null)
            {
                return false;
            }
            else
            {
                return substance.isProcessing;
            }
        }

        /// <summary>
        /// This method creates a temporary object and uses it to create a copy of the original procedural material.
        /// This is useful if you want in runtime create materials that are shared by multiple renderers.
        /// With normal Material class, you can copy materials simpy with the new Material() creator.
        /// Apparently procedural materials have to be instantiated by assigning them to a renderer. They cannot be created/copied like normal materials if you want to keep the procedural properties.
        /// </summary>
        public static ProceduralMaterial CreateProceduralMaterialInstance(this ProceduralMaterial original, ref GameObject tempObject, bool destroyTempObject = true)
        {
            if (tempObject == null)
            {
                tempObject = new GameObject("Procedural Material Holder (Temp Object)");
            }
            var tempRenderer = tempObject.GetOrAddComponent<MeshRenderer>();
            tempRenderer.sharedMaterial = original;
            var instance = tempRenderer.material as ProceduralMaterial;
            if (destroyTempObject)
            {
                UnityEngine.Object.Destroy(tempObject);
            }
            return instance;
        }
    }
}