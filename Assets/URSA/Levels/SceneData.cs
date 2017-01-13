using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
public class SceneData : ScriptableObject
{
    public string levelname;
    public string scene;
    public string NiceName;

    public List<string> entryPoints = new List<string>();

    public override string ToString()
    {
        return scene == "" || scene == null ? "new level" : scene;
    }
}



