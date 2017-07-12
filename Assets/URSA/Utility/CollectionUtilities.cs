namespace URSA.Utility {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public static class CollectionUtilities {

        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;

            }
        }

        public static bool NullOrEmpty<T>(this List<T> list) {
            if (list == null)
                return true;
            if (list.Count == 0)
                return true;
            int c = list.Count;
            for (int i = 0; i < c; i++) {
                if (list[i] == null)
                    continue;
                else
                    return false;
            }
            return true;
        }

        public static V GetValueOrDefault<K, V>(this Dictionary<K, V> dic, K key) {
            V ret;
            bool found = dic.TryGetValue(key, out ret);
            if (found) {
                return ret;
            }
            return default(V);
        }

        public static void Clear<T>(this T[] arr) {
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = default(T);
            }
        }

        public static T RandomEnumValue<T>() {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new System.Random().Next(v.Length));
        }

        public static string[] ListToString<T>(List<T> list) {
            string[] result = new string[list.Count];
            for (int i = 0; i < list.Count; i++) {
                result[i] = list[i].ToString();
            }
            return result;
        }


    }

}