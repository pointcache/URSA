namespace URSA.SceneManagement {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine.SceneManagement;
    using URSA.Utility;
    using URSA.Utility.NotEditableString;

    public class SceneData : ScriptableObject {

        [NotEditableString]
        public string ID;
        [NotEditableString]
        public string scene;
        [NotEditableString]
        public string scenePath;
        [NotEditableString]
        public string sceneManagerPath;

        public string levelname;

        public string NiceName;

        public List<string> entryPoints = new List<string>();

        public override string ToString() {
            return scene == "" || scene == null ? "new level" : scene;
        }
    }
}