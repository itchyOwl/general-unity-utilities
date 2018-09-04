using UnityEngine;
using System;

namespace ItchyOwl.Auxiliary
{
    /// <summary>
    /// Calculates translation from a point to another using translation in a Bezier curve.
    /// Does not move transforms.
    /// </summary>
    public class BezierPositionTweener : PositionTweener
    {
        public Vector3 ControlPoint { get; private set; }

        public void TweenPosition(Vector3 from, Vector3 to, Vector3 controlPoint, float speed, Action<float> updateCallback = null, Action callbackWhenReady = null, Action callbackWhenAborted = null)
        {
            ControlPoint = controlPoint;
            TweenPositionInternal(from, to, speed, updateCallback, callbackWhenReady, callbackWhenAborted);

            //// For debugging (the same method can be used in UI):
            //var visualizer = new GameObject("trajectory");
            //var lineRenderer = visualizer.AddComponent<LineRenderer>();
            //lineRenderer.startWidth = 0.05f;
            //lineRenderer.endWidth = 0.2f;
            //int vertexCount = 10;
            //lineRenderer.positionCount = vertexCount;
            //for (int i = 0; i < vertexCount; i++)
            //{
            //    float t = (float)i / (vertexCount - 1);
            //    Vector3 pos = ItchyOwl.General.Math.Bezier(from, controlPoint, to, t);
            //    lineRenderer.SetPosition(i, pos);
            //}
            //Debug.DrawLine(from, controlPoint, Color.red, 10);
            //Debug.DrawLine(controlPoint, to, Color.red, 10);
            //Debug.DrawLine(from, to, Color.red, 10);
        }

        protected override Vector3 CalculatePosition(float value)
        {
            return ItchyOwl.General.Math.Bezier(StartPoint, ControlPoint, EndPoint, value / Distance);
        }
    }
}