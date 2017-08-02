namespace URSA.Utility {

    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System.IO;
    using System.Collections.Generic;
    using System;
    using URSA.Internal;

    public static class PathUtilities {

        public static string CustomDataPath
        {
            get { return Application.persistentDataPath + "/" + URSASettings.Current.CustomDataFolder; }
        }

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

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }

}