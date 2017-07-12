namespace URSA.SceneManagement {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    using System.Linq;

    public class SceneDataCollector : ScriptableObject {

        static SceneDataCollector _current;
        public static SceneDataCollector Current
        {
            get {
                if (_current == null)
                    _current = Resources.Load(URSAConstants.PATH_ADDITIONAL_DATA + URSAConstants.PATH_URSASETTINGS_SCENE_DATA_COLLECTOR) as SceneDataCollector;
                return _current;
            }
        }


        public List<SceneData> scenes = new List<SceneData>();

        public SceneData GetCurrent() {
            string scene = SceneManager.GetActiveScene().path;
            return scenes.FirstOrDefault(x => x.sceneManagerPath == scene);
        }
    }

}