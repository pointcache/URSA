namespace URSA.Utility {

    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;
    using System;

    public static class PathUtilities {



        public static string DataPathWithoutAssets
        {
            get {
                string datapath = Application.dataPath;
                int index = datapath.IndexOf("/Assets");
                return datapath.Remove(index, 7);
            }
        }

        public static string ClearPathToResources(this string path) {
            int index = path.IndexOf("/Resources/");
            return path.Remove(0, index + 11);
        }

        public static string RemoveExtension(this string path) {
            int index = path.LastIndexOf('.');
            return path.Remove(index, path.Length - index);
        }

#if UNITY_EDITOR
        public static string GetSelectedPathOrFallback() {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
#endif
    }

}