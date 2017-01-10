using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class InitializerSystem : MonoBehaviour
{

    public static bool FullyInitialized { get; private set; }

    GameObject Configs;
    GameObject Systems;

    //Used when we are switching scenes/maps and we need to get notified when this happens, its different from 
    //OnSceneLoaded in that onscene fires every time you hit play, while this fires only when you manually force scene
    //switch, right before it happens, so core systems have a chance to clean up in preparation for new scene
    public static event Action OnReload = delegate { };


    public static event Action PreInitalization = delegate { };
    public static event Action Initialization = delegate { };
    public static event Action PostInitialization = delegate { };
    public static event Action UiInitialization = delegate { };

    bool globalInitialized;

    void OnEnable()
    {
        Configs = transform.Find("Configs").gameObject;
        if (!Configs)
        {
            Debug.LogError("GlobalSystems: No Configs gameobject found");
            return;
        }
        Configs.SetActive(true);

        Systems = transform.Find("Systems").gameObject;
        if (!Systems)
        {
            Debug.LogError("GlobalSystems: No Systems gameobject found");
            return;
        }

        if(Systems.activeSelf)
        {
            Debug.LogError("GlobalSystems: Systems should start DISABLED");
            return;
        }
        Systems.SetActive(true);

        LoadGameConfigs();
        //enable and deserialize global configs
        StartCoroutine(CoreInitializationSequence());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void LoadGameConfigs()
    {
        // TODO : deserialization
    }

    public static void SwitchScene(string name)
    {
        OnReload();
        FullyInitialized = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { StartCoroutine(LevelInitializationSequence()); }

    IEnumerator CoreInitializationSequence()
    {
        for (;;)
        {
            yield return null;
            yield return null;
            Systems.SetActive(true);
            globalInitialized = true;
            yield break;

        }
    }

    IEnumerator LevelInitializationSequence()
    {
        for (;;)
        {
            while (!globalInitialized)
                yield return null;
            yield return null;
            //Enable level configs
            PreInitalization();
            //Deserialize level configs and data 
            LoadSaveFile();
            yield return null;
            //When we are fully ready to start, launch our systems
            Initialization();
            yield return null;
            //Initialize  other systems that require the rest to be already working
            PostInitialization();
            //Launch game loop
            Pool<InternalConfig>.First.UpdateAllowed.Value = true;
            FullyInitialized = true;
            UiInitialization();
            yield break;
        }
    }

    void LoadSaveFile()
    {
        // TODO : level data deserialization
    }
}
