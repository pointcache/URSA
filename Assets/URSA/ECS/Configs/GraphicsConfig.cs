using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;
using URSA;

[Config("Graphics")]
public class GraphicsConfig : ConfigBase {

    [ConsoleVar("gfx.maxframerate", "Maximum allowed framerate")]
    public r_int TargetFramerate = new r_int(300);

    [ConsoleVar("gfx.fov", "Field of view")]
    public r_float fov = new r_float(100f);

    [ConsoleVar("gfx.ao", "Ambient occlusion")]
    public r_bool ao = new r_bool(true);

    [ConsoleVar("gfx.moblur", "Motion blur")]
    public r_bool moblur = new r_bool(true);

    [ConsoleVar("gfx.taa_mode", "Temporal anti-aliasing, off, low, high")]
    public r_TAA_MODE taa_mode = new r_TAA_MODE(TAA_MODE.high);

    [ConsoleVar("gfx.ssr", "Screen space reflections")]
    public r_bool ssr = new r_bool(true);

    [ConsoleVar("gfx.abberations", "Chromatic abberations")]
    public r_bool abberations = new r_bool(true);

    [ConsoleVar("gfx.bloom", "Wide bloom around bright objects")]
    public r_bool bloom = new r_bool(true);

    [ConsoleVar("gfx.flares", "Anamorphic vertical flares")]
    public r_bool flares = new r_bool(true);

    [ConsoleVar("gfx.lensdirt", "Lens dirt on camera")]
    public r_bool lensdirt = new r_bool(true);

    [ConsoleVar("gfx.vignette", "Darker corners of the screen")]
    public r_bool vignette = new r_bool(true);

    [ConsoleVar("gfx.fisheye", "Lens distortion of the screen")]
    public r_bool fisheye = new r_bool(true);

    [ConsoleVar("gfx.noise", "Cinematic noise and grain")]
    public r_bool noise = new r_bool(true);

    [ConsoleVar("gfx.shadows", "Shadow quality, off, low, medium, high")]
    public r_Quality shadows = new r_Quality(Quality.high);

    [ConsoleVar("gfx.softshadows", "Soft shadows, off= hard shadows without filtering")]
    public r_bool softshadows = new r_bool(true);

    public enum TAA_MODE {
        off,
        low,
        high
    }

    public override void OnEnable() {
        base.OnEnable();
    }

    public override void OnDisable() {
        base.OnDisable();
    }
}

public enum Quality {
    off,
    low,
    medium,
    high,
    ultra
}