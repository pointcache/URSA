namespace URSA {
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
            OnUiEnabled,
            OnFullyInitialized
        }

        public virtual void OnEnable() {
            if (!URSAController.FullyInitialized) {
                switch (onEvent) {
                    case InitializationEvent.OnLoadLocalData:
                        URSAController.OnLoadLocalData += DoInitialize;
                        break;
                    case InitializationEvent.OnSystemsEnabled:
                        URSAController.OnSystemsEnabled += DoInitialize;
                        break;
                    case InitializationEvent.OnFullyInitialized:
                        URSAController.OnFullyInitialized += DoInitialize;
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
                    URSAController.OnLoadLocalData -= DoInitialize;
                    break;
                case InitializationEvent.OnSystemsEnabled:
                    URSAController.OnSystemsEnabled -= DoInitialize;
                    break;
                case InitializationEvent.OnFullyInitialized:
                    URSAController.OnFullyInitialized -= DoInitialize;
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