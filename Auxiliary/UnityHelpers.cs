using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace ItchyOwl.Auxiliary
{
    public static class UnityHelpers
    {
        #region Layers
        public static int GetLayer(string layerName)
        {
            return LayerMask.NameToLayer(layerName);
        }

        public static int GetNavLayer(string layerName)
        {
            return UnityEngine.AI.NavMesh.GetAreaFromName(layerName);
        }
        #endregion

        #region Raycasting
        public static bool RaycastMousePos(out RaycastHit hit)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, Mathf.Infinity);
        }

        public static bool RaycastMousePos(int layerMask, out RaycastHit hit)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
        }

        public static bool RaycastMousePos(int layerMask)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, Mathf.Infinity, layerMask);
        }

        public static bool RaycastMousePos(Collider collider, out RaycastHit hit)
        {
            if (collider == null) { throw new Exception("[UnityHelpers] Collider null"); }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return collider.Raycast(ray, out hit, Mathf.Infinity);
        }

        public static RaycastHit[] RaycastMousePosAll(int layerMask)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
        }

        public static bool RaycastScreenPointToRay(Vector2 screenPoint, out RaycastHit hit, Camera camera = null)
        {
            camera = camera == null ? Camera.main : camera;
            Ray ray = RectTransformUtility.ScreenPointToRay(camera, screenPoint);
            return Physics.Raycast(ray, out hit, Mathf.Infinity);
        }
        #endregion

        #region Transforms
        /// <summary>
        /// Distributes positions from the given array of positions to the transforms. Nulls are allowed in the transform array.
        /// </summary>
        public static bool PositionTransformsInOrder(Transform[] transforms, Vector3[] positions, bool recyclePositions, bool isLocal)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];
                if (t == null) { continue; }
                int positionIndex = i;
                if (positionIndex >= positions.Length)
                {
                    if (recyclePositions)
                    {
                        positionIndex = 0;
                    }
                    else
                    {
                        Debug.LogWarningFormat("[Helpers] Not enough positions for all transforms and recycling is set false. Ignoring transforms that are left over. Positions: {0}, transforms: {1}", positions.Length, transforms.Length);
                        return false;
                    }
                }
                if (isLocal)
                {
                    t.localPosition = positions[positionIndex];
                }
                else
                {
                    t.position = positions[positionIndex];
                }
            }
            return true;
        }

        /// <summary>
        /// Distributes orientations (local euler angles) from the given array of orientations to the transforms.
        /// </summary>
        public static bool OrientTransformsInOrder(Transform[] transforms, Vector3[] orientations, bool recycleOrientations, bool isLocal)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];
                int orientationIndex = i;
                if (orientationIndex >= orientations.Length)
                {
                    if (recycleOrientations)
                    {
                        orientationIndex = 0;
                    }
                    else
                    {
                        Debug.LogWarningFormat("[Helpers] Not enough orientations for all transforms and recycling is set false. Ignoring transforms that are left over. Orientations: {0}, transforms: {1}", orientations.Length, transforms.Length);
                        return false;
                    }
                }
                if (isLocal)
                {
                    t.localEulerAngles = orientations[i];
                }
                else
                {
                    t.eulerAngles = orientations[i];
                }
            }
            return true;
        }

        public static bool PositionTransformsInOrder(IEnumerable<Transform> transforms, IEnumerable<Vector3> positions, bool recyclePositions, bool isLocal)
        {
            return PositionTransformsInOrder(transforms.ToArray(), positions.ToArray(), recyclePositions, isLocal);
        }

        public static bool OrientTransformsInOrder(IEnumerable<Transform> transforms, IEnumerable<Vector3> orientations, bool recycleOrientations, bool isLocal)
        {
            return OrientTransformsInOrder(transforms.ToArray(), orientations.ToArray(), recycleOrientations, isLocal);
        }
        #endregion

        #region Vectors
        public static Vector3 RandomVector3(float min, float max)
        {
            return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        }

        public static Vector2 RandomVector2(float min, float max)
        {
            return new Vector2(Random.Range(min, max), Random.Range(min, max));
        }

        public static Vector3 InfiniteVector { get { return new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity); } }
        #endregion
    }
}
