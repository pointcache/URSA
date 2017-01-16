
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using URSA;
[InitializeOnLoad]
public static class MySceneDataCreator  {

    static MySceneDataCreator() {
        LevelsSystemEditor.OnSceneDataCreated += CustomSceneDataCreator;
    }

    static void CustomSceneDataCreator(SceneAsset scene, SceneData data) {

        //Collecting entry points from the scene
        //data.creationDate = DateTime.Now;
        //Debug.Log(data.name); 
    }
}
