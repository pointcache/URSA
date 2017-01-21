using UnityEngine;
using System;
using System.Collections.Generic;

public class SceneDataCollector : ScriptableObject {

    static SceneDataCollector _current;
    public static SceneDataCollector current
    {
        get {
            if (_current == null)
                _current = Resources.Load(URSAConstants.PATH_ADDITIONAL_DATA + URSAConstants.PATH_URSASETTINGS_SCENE_DATA_COLLECTOR) as SceneDataCollector;
            return _current;
        }
    }


    public List<SceneData> scenes = new List<SceneData>();
}
