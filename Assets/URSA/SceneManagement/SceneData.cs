namespace URSA.SceneManagement {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine.SceneManagement;
    using URSA.Utility;

    public class SceneData : ScriptableObject {

#if UNITY_EDITOR
        [NotEditableInt]
#endif 
        public int ID;
#if UNITY_EDITOR
        [NotEditableString]
#endif 
        public string scene;
#if UNITY_EDITOR
        [NotEditableString]
#endif 
        public string scenePath;
#if UNITY_EDITOR
        [NotEditableString]
#endif 
        public string sceneManagerPath;

        public string levelname;

        public string NiceName;

        public List<string> entryPoints = new List<string>();

        public override string ToString() {
            return scene == "" || scene == null ? "new level" : scene;
        }
    }
}