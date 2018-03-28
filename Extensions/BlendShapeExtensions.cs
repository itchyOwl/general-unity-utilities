using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace ItchyOwl.Extensions
{
    public static class BlendShapeExtensions
    {
        public static void ResetAllBlendShapes(this SkinnedMeshRenderer renderer)
        {
            SetAllBlendShapes(renderer, () => 0);
        }

        public static void RandomizeAllBlendShapes(this SkinnedMeshRenderer renderer)
        {
            SetAllBlendShapes(renderer, () => Random.Range(0, 100));
        }

        public static void SetAllBlendShapes(this SkinnedMeshRenderer renderer, Func<float> method)
        {
            for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
            {
                renderer.SetBlendShapeWeight(i, method());
            }
        }

        public static void SetBlendShapes(this SkinnedMeshRenderer renderer, string name, Func<float> method, string[] ignoreWithPhrases = null, bool requireExactMatch = false)
        {
            for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
            {
                string blendShapeName = renderer.sharedMesh.GetBlendShapeName(i);
                if (ignoreWithPhrases != null && ignoreWithPhrases.Any(p => blendShapeName.Contains(p))) { continue; }
                if (requireExactMatch && blendShapeName == name)
                {
                    renderer.SetBlendShapeWeight(i, method());
                }
                else if (!requireExactMatch && blendShapeName.Contains(name))
                {
                    renderer.SetBlendShapeWeight(i, method());
                }
            }
        }

        public static float GetBlendShapeWeight(this SkinnedMeshRenderer renderer, string blendShapeName)
        {
            int index = renderer.sharedMesh.GetBlendShapeIndex(blendShapeName);
            return renderer.GetBlendShapeWeight(index);
        }

        public static List<string> GetAllBlendShapeNames(this Mesh mesh)
        {
            var names = new List<string>();
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                names.Add(mesh.GetBlendShapeName(i));
            }
            return names;
        }

        public static List<float> GetAllBlendShapeWeights(this SkinnedMeshRenderer renderer)
        {
            var weights = new List<float>();
            for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
            {
                weights.Add(renderer.GetBlendShapeWeight(i));
            }
            return weights;
        }

        public static Dictionary<string, float> GetAllBlendShapes(this SkinnedMeshRenderer renderer)
        {
            var allNames = GetAllBlendShapeNames(renderer.sharedMesh);
            var blendShapes = new Dictionary<string, float>();
            for (int i = 0; i < allNames.Count; i++)
            {
                blendShapes.Add(allNames[i], renderer.GetBlendShapeWeight(i));
            }
            return blendShapes;
        }

        /// <summary>
        /// Copy blend shape frames from the source and create them on the target.
        /// Works only on identical meshes.
        /// Pressupposes that the target does not have blend shapes. Not tested on targets that already have shapes.
        /// </summary>
        public static void CopyBlendShapesTo(this Mesh source, Mesh target)
        {
            int vertexCount = source.vertexCount;
            if (target.vertexCount > vertexCount)
            {
                Debug.LogWarningFormat("[BlendShapeExtensions] The target {0} has more vertices than the source {1}", target.name, source.name);
                return;
            }
            else if (target.vertexCount < vertexCount)
            {
                Debug.LogWarningFormat("[BlendShapeExtensions] The target {0} has less vertices than the source {1}", target.name, source.name);
                return;
            }
            var deltaVertices = new Vector3[vertexCount];
            var deltaNormals = new Vector3[vertexCount];
            var deltaTangents = new Vector3[vertexCount];
            for (int shape = 0; shape < source.blendShapeCount; shape++)
            {
                string name = source.GetBlendShapeName(shape);
                for (int frame = 0; frame < source.GetBlendShapeFrameCount(shape); frame++)
                {
                    float weight = source.GetBlendShapeFrameWeight(shape, frame);
                    source.GetBlendShapeFrameVertices(shape, frame, deltaVertices, deltaNormals, deltaTangents);
                    target.AddBlendShapeFrame(name, weight, deltaVertices, deltaNormals, deltaTangents);
                }
            }
        }
    }
}