namespace URSA.ECS.Initialization {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Behavior Tied to initialization pipe through Initialize() 
    /// </summary>
    public class InitializedBehavior : MonoBehaviour {

        protected bool Initialized { get; set; }

        [SerializeField]
        private InitializationEvent onEvent = InitializationEvent.OnSystemsEnabled;
        private enum InitializationEvent {
            OnLoadLocalData,
            OnSystemsEnabled,
            OnFullyInitialized
        }

        protected virtual void OnEnable() {
            if (!ECSController.FullyInitialized) {
                switch (onEvent) {
                    case InitializationEvent.OnLoadLocalData:
                        ECSController.OnLoadLocalData += DoInitialize;
                        break;
                    case InitializationEvent.OnSystemsEnabled:
                        ECSController.OnSystemsEnabled += DoInitialize;
                        break;
                    case InitializationEvent.OnFullyInitialized:
                        ECSController.OnFullyInitialized += DoInitialize;
                        break;
                    default:
                        break;
                }
            }
            else
                DoInitialize();
        }

        protected virtual void OnDisable() {
            switch (onEvent) {
                case InitializationEvent.OnLoadLocalData:
                    ECSController.OnLoadLocalData -= DoInitialize;
                    break;
                case InitializationEvent.OnSystemsEnabled:
                    ECSController.OnSystemsEnabled -= DoInitialize;
                    break;
                case InitializationEvent.OnFullyInitialized:
                    ECSController.OnFullyInitialized -= DoInitialize;
                    break;
                default:
                    break;
            }
        }

        protected void DoInitialize() {
            Initialize();
            Initialized = true;
        }

        protected virtual void Initialize() {

        }
    }

}