namespace URSA.ECS.Initialization {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Behavior Tied to initialization pipe through Initialize() 
    /// </summary>
    public class InitializedBehavior : MonoBehaviour {

        public InitializationEvent onEvent = InitializationEvent.OnSystemsEnabled;
        public enum InitializationEvent {
            OnLoadLocalData,
            OnSystemsEnabled,
            OnFullyInitialized
        }

        public virtual void OnEnable() {
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

        public virtual void OnDisable() {
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

        void DoInitialize() {
            Initialize();
        }

        public virtual void Initialize() {

        }
    }

}