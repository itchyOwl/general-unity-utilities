using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using ItchyOwl.Extensions;

namespace ItchyOwl.General
{
    public static class Math
    {
        public static bool IsValid(float value)
        {
            return (!float.IsInfinity(value) && !float.IsNaN(value));
        }

        /// <summary>
        /// Float comparison. Note that may still fail in some cases.
        /// References: 
        /// http://floating-point-gui.de/errors/comparison/
        /// https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
        /// NOTE: This is a custom solution, similar to Mathf.Approximately.
        /// </summary>
        public static bool NearlyEqual(float a, float b, float epsilon = 0.0001f)
        {
            float diff = Mathf.Abs(a - b);
            if (a == b)
            {
                // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < float.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }
            else
            {
                // use relative error
                return diff / (Mathf.Abs(a) + Mathf.Abs(b)) < epsilon;
            }
        }

        /// <summary>
        /// Calculates a random value based on the given probability weights.
        /// Use inverse lerp and lerp to get a value between the min and max you want.
        /// Example:
        /// float random = Math.GetWeightedRandom(weigths);
        /// float inverseLerp = Mathf.InverseLerp(weigths.First(), weigths.Last(), random);
        /// float value = Mathf.Lerp(0, 100, inverseLerp);
        /// </summary>
        public static float GetWeightedRandom(IEnumerable<float> weights)
        {
            // 1) calculate the sum of all the weights
            // 2) pick a random number that is 0 or greater and is less than the sum of the weights
            // 3) go through the items one at a time, subtracting their weight from your random number, until you get the item where the random number is less than that item's weight
            // Modified from: http://stackoverflow.com/questions/1761626/weighted-random-numbers.
            float sum = weights.Sum();
            float random = Random.Range(0, sum);
            foreach (var weight in weights)
            {
                if (random < weight)
                {
                    return weight;
                }
                else
                {
                    random -= weight;
                }
            }
            return 0;   // Should not get here
        }

        /// <summary>
        /// Calculates a random value based on the given probability weights.
        /// The random value is lerped between 0 and 1.
        /// </summary>
        public static float GetWeightedRandom01(IEnumerable<float> weights)
        {
            float random = GetWeightedRandom(weights);
            float inverseLerp = Mathf.InverseLerp(weights.First(), weights.Last(), random);
            return Mathf.Lerp(0, 1, inverseLerp);
        }

        /// <summary>
        /// Calculates a random value based on the given probability weights.
        /// The random value is lerped between 0 and 100.
        /// </summary>
        public static float GetWeightedRandom0100(IEnumerable<float> weigths)
        {
            float random = GetWeightedRandom(weigths);
            float inverseLerp = Mathf.InverseLerp(weigths.First(), weigths.Last(), random);
            return Mathf.Lerp(0, 100, inverseLerp);
        }

        /// <summary>
        /// For GetWeightedRandom function
        /// </summary>
        public static float[] GetExponentiallyDecreasingWeights(int weightsCount, float exponent = 2)
        {
            var weights = new float[weightsCount];
            for (int i = 0; i < weightsCount; i++)
            {
                weights[i] = Mathf.Pow(weightsCount, exponent) - Mathf.Pow(i + 1, exponent);
            }
            return weights;
        }

        /// <summary>
        /// For GetWeightedRandom function
        /// </summary>
        public static float[] GetLinearilyDecreasingWeights(int weightsCount)
        {
            var weights = new float[weightsCount];
            for (int i = 0; i < weightsCount; i++)
            {
                weights[i] = weightsCount - i;
            }
            return weights;
        }

        /// <summary>
        /// Treats the probability as a percentage between 0 and 100 and checks whether or not it realizes.
        /// </summary>
        public static bool IsProbabilityRealized(float probability)
        {
            return Lerp01(0, 100, probability) + Random.value > 1;
        }

        /// <summary>
        /// Turns the value between min and max into a probability and checks whether or not it realizes.
        /// </summary>
        public static bool IsProbabilityRealized(float min, float max, float probability)
        {
            return Lerp01(min, max, probability) + Random.value > 1;
        }

        /// <summary>
        /// Calculates a lerp between 0 and 1.
        /// </summary>
        public static float Lerp01(float min, float max, float current)
        {
            float inverseLerp = Mathf.InverseLerp(min, max, current);
            return Mathf.Lerp(0, 1, inverseLerp);
        }

        public static float Average(float value1, float value2)
        {
            return (value1 + value2) / 2;
        }

        /// <summary>
        /// For more than three values, use IEnumerable.Average (found in System.Linq)
        /// </summary>
        public static float Average(float value1, float value2, float value3)
        {
            return (value1 + value2 + value3) / 3;
        }

        /// <summary>
        /// The average of the weights should be 1.
        /// </summary>
        public static float WeightedAvg(float value1, float value2, float weight1, float weight2)
        {
            return (value1 * weight1 + value2 * weight2) / 2;
        }

        /// <summary>
        /// The average of the weights should be 1.
        /// </summary>
        public static float WeightedAvg(float value1, float value2, float value3, float weight1, float weight2, float weight3)
        {
            return (value1 * weight1 + value2 * weight2 + value3 * weight3) / 3;
        }

        /// <summary>
        /// The average of the weights should be 1.
        /// </summary>
        public static float WeightedAvg(float[] values, float[] weights)
        {
            if (values.Length > weights.Length || values.Length < weights.Length)
            {
                Debug.LogWarning("[Math] values length is not equal to the weights length. Returning zero!");
                return 0;
            }
            float sum = 0;
            int length = values.Length;
            for (int i = 0; i < length; i++)
            {
                sum += values[i] * weights[i];
            }
            return sum / length;
        }

        public static float ToRadians(float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }

        public static float ToDegrees(float radians)
        {
            return radians * Mathf.Rad2Deg;
        }

        /// <summary>
        /// portion / total * 100. 
        /// </summary>
        public static float Percentage(float portion, float total)
        {
            return portion / total * 100;
        }

        /// <summary>
        /// percentage / 100.
        /// </summary>
        public static float FractionFromPercentage(float percentage)
        {
            return percentage / 100;
        }

        /// <summary>
        /// fraction * 100.
        /// </summary>
        public static float PercentageFromFraction(float fraction)
        {
            return fraction * 100;
        }

        public static Vector2 RandomPointInCircleEdge(Vector2 center, float radius, float thickness = 0)
        {
            var randomPoint = center + RandomPointInCircle().normalized * radius;
            if (thickness > 0)
            {
                randomPoint = randomPoint.TranslatePointTowardPoint(center, Random.Range(-thickness, thickness));
            }
            return randomPoint;
        }

        /// <summary>
        /// Returns a random point inside a specific circle.
        /// </summary>
        public static Vector2 RandomPointInCircle(Vector2 center, float radius)
        {
            return center + radius * RandomPointInCircle();
        }

        /// <summary>
        /// Returns a random point inside a circle with a diameter between 0 and 1.
        /// </summary>
        public static Vector2 RandomPointInCircle()
        {
            // Let's use the implementation now found in the UnityEngine.Random class.
            return Random.insideUnitCircle;
            //float t = 2f * Mathf.PI * Random.value;
            //float u = Random.value + Random.value;
            //float r = (u > 1f) ? (2f - u) : u;
            //float x = r * Mathf.Cos(t);
            //float y = r * Mathf.Sin(t);
            //return new Vector2(x, y);
        }

        /// <summary>
        /// Rotates a point in 2d space around another point.
        /// Modified from:
        /// http://www.gamefromscratch.com/post/2012/11/24/GameDev-math-recipes-Rotating-one-point-around-another-point.aspx
        /// </summary>
        public static Vector2 RotatePointAroundTarget(Vector2 point, Vector2 target, float degrees, bool clockWise = true)
        {
            // (Mathf.PI / 180) * degrees
            var angle = ToRadians(degrees);
            var sin = Mathf.Sin(angle);
            var cos = Mathf.Cos(angle);
            if (clockWise)
            {
                sin = -sin;
            }
            Vector2 dir = point - target;
            var x = (cos * dir.x) - (sin * dir.y) + target.x;
            var y = (sin * dir.x) + (cos * dir.y) + target.y;
            return new Vector2(x, y);
        }

        public static float InterpolateBetween(float srcValue, float srcMin, float srcMax, float dstMin, float dstMax)
        {
            float t = (srcMax - srcValue) / (srcMax - srcMin);
            if (srcMax < srcMin)
            {
                t = 1.0f - t;
            }
            return (1.0f - t) * dstMax + t * dstMin;
        }

        /// <summary>
        /// Returns a position in a curve.
        /// </summary>
        public static Vector3 Bezier(Vector3 start, Vector3 control, Vector3 end, float t)
        {
            return Mathf.Pow(1 - t, 2) * start + 2 * t * (1 - t) * control + Mathf.Pow(t, 2) * end;
        }

        /// <summary>
        /// Returns a position in a curve.
        /// </summary>
        public static Vector2 Bezier(Vector2 start, Vector2 control, Vector2 end, float t)
        {
            return Mathf.Pow(1 - t, 2) * start + 2 * t * (1 - t) * control + Mathf.Pow(t, 2) * end;
        }

        // If we want more control points, try this (not tested):
        //public static Vector3 Bezier3(Vector3 s, Vector3 st, Vector3 et, Vector3 e, float t)
        //{
        //    return (((-s + 3 * (st - et) + e) * t + (3 * (s + et) - 6 * st)) * t + 3 * (st - s)) * t + s;
        //}

        #region Smoothing functions, source https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
        public static float SmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
        }

        public static float SmootherStep(float t)
        {
            return t = t * t * t * (t * (6f * t - 15f) + 10f);
        }

        public static float EaseIn(float t)
        {
            return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
        }

        public static float EaseOut(float t)
        {
            return Mathf.Sin(t * Mathf.PI * 0.5f);
        }

        public static float Exponential(float t)
        {
            return t * t;
        }
        #endregion
    }
}
