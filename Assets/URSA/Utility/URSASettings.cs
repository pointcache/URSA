using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class URSASettings : ScriptableObject {

    [Header("Systems")]
    public GameObject CustomGlobalSystemsPrefab;
    public GameObject GlobalSystemsTemplate;
    public GameObject CustomSceneSystemsPrefab;
    public GameObject SceneSystemsTemplate;

    [Header("Paths")]
    public string DatabaseRootFolder = "ENTITY";
}
