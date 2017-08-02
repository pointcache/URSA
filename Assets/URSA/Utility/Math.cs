namespace URSA.Utility {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public static class Math {

        public static float Remap(this float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }


        public static double Remap(this double value, double from1, double to1, double from2, double to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static float WrapNumber(float value, float min, float max) {
            float result;
            if (value < min)
                result = max - (min - value) % (max - min);
            else
                result = min + (value - min) % (max - min);

            return result;
        }



        public static Vector3 WithX(this Vector3 v, float x) {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float y) {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 WithZ(this Vector3 v, float z) {
            return new Vector3(v.x, v.y, z);
        }

        /// <summary>
        /// Negative values to subtract
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 AddFloats(this Vector3 vec, float x, float y, float z) {
            vec.x += x;
            vec.y += y;
            vec.z += z;
            return vec;
        }
        public static Vector3 AddFloats(this Vector3 vec, float val) {
            return AddFloats(vec, val, val, val);
        }
        public static Vector3 MultiplyFloats(this Vector3 vec, float x, float y, float z) {
            vec.x *= x;
            vec.y *= y;
            vec.z *= z;
            return vec;
        }

        public static Vector3 MultiplyFloat(this Vector3 vec, float x) {
            vec.x *= x;
            vec.y *= x;
            vec.z *= x;
            return vec;
        }

        public static Vector3 DivideFloats(this Vector3 vec, float x, float y, float z) {
            if (x == 0 || y == 0 || z == 0) {
                Debug.LogError("Top lel.");
            }
            vec.x /= x;
            vec.y /= y;
            vec.z /= z;
            return vec;
        }

        public static Vector3 GetPointBetween(Vector3 a, Vector3 b) {
            return (a + b) / 2f;
        }

        //Converts 3d direction vector into 2d, basically flattens it
        public static Vector3 ConvertTo2dCoords(this Vector3 vec) {
            float mag = vec.magnitude;
            vec.y = 0;
            return vec.normalized * mag;
        }

        //Converts 3d direction vector into 2d, basically flattens it
        public static Vector3 ConvertTo2dCoordsNoY(this Vector3 vec) {
            vec.y = 0f;
            float mag = vec.magnitude;
            vec = vec.normalized;
            vec.y = 0;
            return vec * mag;
        }

        //Converts 3d direction vector into 2d, basically flattens it
        public static Vector2 MakeV2(this Vector3 vec) {
            return new Vector2(vec.x, vec.y);
        }

        //Imagine two circles inside each other, this limits vector to only area in between the two
        public static Vector3 LimitRadiusArea(this Vector3 vec, float min, float max) {
            vec.x = _limiradiusArea(vec.x, max, min);
            vec.y = _limiradiusArea(vec.y, max, min);
            vec.z = _limiradiusArea(vec.z, max, min);
            return vec;
        }
        private static float _limiradiusArea(float _in, float max, float min) {
            if (_in > 0) {
                if (_in > max)
                    _in = max;
                if (_in < min)
                    _in = min;
            }
            else {
                if (_in < max * -1)
                    _in = max * -1;
                if (_in > min * -1)
                    _in = min * -1;
            }
            return _in;
        }

        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n) {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        //Converts vector2 to vector3 so that x = x, but y = z, basically from 2d plane to proper 3d
        public static Vector3 ToVector3d(this Vector2 vec) {
            return new Vector3(vec.x, 0, vec.y);
        }

        //Converts vector2 to vector3 so that x = x, but y = z, basically from 2d plane to proper 3d also sets Y value
        public static Vector3 ToVector3d(this Vector2 vec, float Y) {
            return new Vector3(vec.x, Y, vec.y);
        }

        #region Multilerp
        // This is not very optimized. There are at least n + 1 and at most 2n Vector3.Distance
        // calls (where n is the number of waypoints). 
        public static Vector3 MultiLerp(Vector3[] waypoints, float ratio) {
            Vector3 position = Vector3.zero;
            float totalDistance = waypoints.MultiDistance();
            float distanceTravelled = totalDistance * ratio;

            int indexLow = GetVectorIndexFromDistanceTravelled(waypoints, distanceTravelled);
            int indexHigh = indexLow + 1;

            // we're done
            if (indexHigh > waypoints.Length - 1)
                return waypoints[waypoints.Length - 1];


            // calculate the distance along this waypoint to the next
            Vector3[] completedWaypoints = new Vector3[indexLow + 1];

            for (int i = 0; i < indexLow + 1; i++) {
                completedWaypoints[i] = waypoints[i];
            }

            float distanceCoveredByPreviousWaypoints = completedWaypoints.MultiDistance();
            float distanceTravelledThisSegment = distanceTravelled - distanceCoveredByPreviousWaypoints;
            float distanceThisSegment = Vector3.Distance(waypoints[indexLow], waypoints[indexHigh]);

            float currentRatio = distanceTravelledThisSegment / distanceThisSegment;
            position = Vector3.Lerp(waypoints[indexLow], waypoints[indexHigh], currentRatio);

            return position;
        }

        public static float MultiDistance(this Vector3[] waypoints) {
            float distance = 0f;

            for (int i = 0; i < waypoints.Length; i++) {
                if (i + 1 > waypoints.Length - 1)
                    break;

                distance += Vector3.Distance(waypoints[i], waypoints[i + 1]);
            }

            return distance;
        }

        public static int GetVectorIndexFromDistanceTravelled(Vector3[] waypoints, float distanceTravelled) {
            float distance = 0f;

            for (int i = 0; i < waypoints.Length; i++) {
                if (i + 1 > waypoints.Length - 1)
                    return waypoints.Length - 1;

                float segmentDistance = Vector3.Distance(waypoints[i], waypoints[i + 1]);

                if (segmentDistance + distance > distanceTravelled) {
                    return i;
                }

                distance += segmentDistance;
            }

            return waypoints.Length - 1;
        }
        #endregion
    }
}