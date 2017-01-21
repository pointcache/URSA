using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class InitializerSystem : MonoBehaviour {

    public static bool FullyInitialized { get; private set; }


    //Used when we are switching scenes/maps and we need to get notified when this happens, its different from 
    //OnSceneLoaded in that onscene fires every time you hit play, while this fires only when you manually force scene
    //switch, right before it happens, so core systems have a chance to clean up in preparation for new scene
    public static event Action OnManualSceneSwitch = delegate { };

    /// <summary>
    /// Hook your config loading here, it will happen before global systems are initialized
    /// </summary>
    public static event Action OnGlobalLoadConfigs = delegate { };
    /// <summary>
    /// Hook your configuration events that use configs, happens after configs were deserialized
    /// </summary>
    public static event Action OnApplicationConfiguration = delegate { };
    /// <summary>
    /// Deserialize your initial player profiles, etc
    /// </summary>
    public static event Action OnInitialLoadPersistentData = delegate { };
    /// <summary>
    /// Hook your configuration events that use configs, happens after configs were deserialized
    /// </summary>
    public static event Action OnGlobalSystemsEnabled = delegate { };
    /// <summary>
    /// Hook your save loading here, it will happen before local systems and loaders are initialized
    /// </summary>
    public static event Action OnLoadLocalData = delegate { };
    /// <summary>
    /// Hook your level creation/generation/object spawning here
    /// </summary>
    public static event Action OnLoadersEnabled = delegate { };
    /// <summary>
    /// Will enable LocalSystems stack
    /// </summary>
    public static event Action OnSystemsEnabled = delegate { };
    /// <summary>
    /// Hook your ui here
    /// </summary>
    public static event Action OnUiEnabled = delegate { };

    bool globalInitialized;

    void OnEnable() {

        SceneManager.sceneLoaded += OnSceneLoaded;

        StartCoroutine(GlobalInitializationSequence());

    }

    public static void LoadLevel(SceneData transition) {
        OnManualSceneSwitch();
        FullyInitialized = false;

        string scene = transition.scenePath;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public static void LoadLevel(LevelTransition transition) {
        OnManualSceneSwitch();
        FullyInitialized = false;

        string scene = transition.scenedata.scenePath;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        StartCoroutine(LocalInitializationSequence());
    }

    IEnumerator GlobalInitializationSequence() {
        for (;;) {
            OnGlobalLoadConfigs();
            yield return null;
            OnApplicationConfiguration();
            yield return null;
            OnInitialLoadPersistentData();
            yield return null;
            OnGlobalSystemsEnabled();
            globalInitialized = true;
            yield break;

        }
    }

    IEnumerator LocalInitializationSequence() {
        for (;;) {
            while (!globalInitialized)
                yield return null;
            yield return null;
            OnLoadLocalData();
            yield return null;
            OnLoadersEnabled();
            yield return null;
            OnSystemsEnabled();
            //Launch game loop 
            Pool<InternalConfig>.First.UpdateAllowed.Value = true;
            FullyInitialized = true;
            OnUiEnabled();
            yield break;
        }
    }
}
