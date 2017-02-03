using UnityEngine;
using System;
using System.Collections.Generic;

public class ConfiguratorSystem : MonoBehaviour {

    bool shadowsstate;
    bool softshadows;

    void Awake() {
        InitializerSystem.OnApplicationConfiguration += ConfigureGraphics;
        InitializerSystem.OnLoadersEnabled += SetShadowsOnTheLevel;
    }

    void ConfigureGraphics() {
        var cfg = Pool<GraphicsConfig>.First;


        cfg.TargetFramerate.SubAndUpdate(x => Application.targetFrameRate = x);
        cfg.softshadows.SubAndUpdate(x => { softshadows = x; SetShadows(cfg.shadows); });
        cfg.shadows.SubAndUpdate(SetShadows);
    }

    void SetShadowsOnTheLevel() {
        var lights = FindObjectsOfType<Light>();

        foreach (var light in lights) {
            var init = light.GetComponent<LightInitState>();
            if (!init) {
                init = light.gameObject.AddComponent<LightInitState>();
                init.init = light.shadows;
            }

            if (init.init == LightShadows.None)
                return;

            if (!shadowsstate)
                light.shadows = LightShadows.None;
            else {
                if (softshadows)
                    light.shadows = LightShadows.Soft;
                else
                    light.shadows = LightShadows.Hard;
            }
        }
    }

    void SetShadows(Quality quality) {
        switch (quality) {
            case Quality.off:
                shadowsstate = false;
                break;
            case Quality.low:
                shadowsstate = true;
                QualitySettings.shadowResolution = ShadowResolution.Low;
                break;
            case Quality.medium:
                shadowsstate = true;
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                break;
            case Quality.high:
                shadowsstate = true;
                QualitySettings.shadowResolution = ShadowResolution.High;
                break;
            case Quality.ultra:
                shadowsstate = true;
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                break;
            default:
                break;
        }
        SetShadowsOnTheLevel();
    }
}
