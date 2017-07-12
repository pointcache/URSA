namespace URSA {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    using System.Collections;
    using URSA.SceneManagement;
    using URSA.ECS;
    using URSA.ECS.Systems;
    using URSA.Utility;
    using URSA.Database;
    using URSA.Configurator;
    using URSA.ECS.Components;

    public class ECSController : MonoBehaviour {

        #region SINGLETON
        private static ECSController _instance;
        public static ECSController instance
        {
            get {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<ECSController>();
                return _instance;
            }
        }
        #endregion



        public static ECSController current;

        [SerializeField]
        private GameObject Configs;

        private IConfiguratorSystem Configurator;

        public static bool FullyInitialized { get; private set; }

        //Used when we are switching scenes/maps and we need to get notified when this happens, its different from 
        //OnSceneLoaded in that onscene fires every time you hit play, while this fires only when you manually force scene
        //switch, right before it happens, so core systems have a chance to clean up in preparation for new scene
        public static event Action OnSceneSwitch = delegate { };


        /// <summary>
        /// Deserialize your initial player profiles, etc
        /// </summary>
        public static event Action OnInitialLoadPersistentData = delegate { };

        public static event Action OnGlobalSystemsFullyInitialized = delegate { };
        /// <summary>
        /// Hook your save loading here, it will happen before local systems and loaders are initialized
        /// </summary>
        public static event Action OnLoadLocalData = delegate { };

        /// <summary>
        /// After all local systems were enabled
        /// </summary>
        public static event Action OnSystemsEnabled = delegate { };

        /// <summary>
        /// Hook your ui here
        /// </summary>
        public static event Action OnFullyInitialized = delegate { };
        bool globalInitialized;

        void OnEnable() {

            if (current) {
                Destroy(gameObject);
                return;
            }
            else {
                current = this;
                DontDestroyOnLoad(gameObject);
            }

            Configurator = gameObject.GetInterface<IConfiguratorSystem>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            GlobalInitialization();
        }

        public static void LoadLevel(SceneData transition) {
            OnSceneSwitch();
            FullyInitialized = false;

            string scene = transition.scenePath;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }

        public static void LoadLevel(SceneTransition transition) {
            OnSceneSwitch();
            FullyInitialized = false;

            string scene = transition.scenedata.scenePath;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }

        public static void InitializeObject(Action ToInitialize) {
            if (FullyInitialized) {
                ToInitialize();
                return;
            }
            else {
                OnFullyInitialized -= ToInitialize;
                OnFullyInitialized += ToInitialize;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            LocalInitialization();
        }

        private void GlobalInitialization() {
            if (globalInitialized)
                return;

            if (Configs)
                Configs.SetActive(true);
            gameObject.AddComponent<EntityDatabase>();

            foreach (Transform tr in transform) {
                if (tr.gameObject == Configs)
                    continue;
                if (tr.gameObject.activeSelf) {
                    Debug.LogError("No direct children of InitializerSystem can be active before runtime.");
                    Application.Quit();
                }
                tr.gameObject.SetActive(true);
            }


            OnInitialLoadPersistentData();
            if (Configurator != null)
                Configurator.ConfigureGlobal();
            globalInitialized = true;
            var GlobalExecutor = gameObject.AddComponent<GlobalSystemsExecutor>();
            OnGlobalSystemsFullyInitialized();
        }

        private void LocalInitialization() {
            if (!globalInitialized)
                GlobalInitialization();

            OnLoadLocalData();

            var localSystems = GameObject.FindObjectOfType<SystemsExecutor>();
            if (localSystems) {
                localSystems.RunOrderedOnEnable();
            }
            else {
                Debug.Log("Local Systems were not found");
            }

            OnSystemsEnabled();
            if (Configurator != null)
                Configurator.ConfigureLocal();
            FullyInitialized = true;
            OnFullyInitialized();
            SystemsExecutor.UpdateAllowed = true;
        }
    }

}