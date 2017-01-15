using UnityEngine;
using System;
using System.Collections.Generic;

public class ConfiguratorSystem : MonoBehaviour {

    void Awake()
    {
       InitializerSystem.OnApplicationConfiguration += ConfigureGraphics;
    }

    void ConfigureGraphics()
    {
        var cfg = Pool<GraphicsConfig>.First;

        
        cfg.TargetFramerate.OnChanged += x => Application.targetFrameRate = x;
        Application.targetFrameRate = cfg.TargetFramerate;
    }
}
