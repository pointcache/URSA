namespace URSA {

    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class InitializedComponent : ComponentBase {

        public InitializationEvent onEvent = InitializationEvent.OnSystemsEnabled;
        public enum InitializationEvent {
            OnLoadLocalData,
            OnSystemsEnabled,
            OnUiEnabled
        }

        public override void OnEnable() {
            base.OnEnable();
            if (!URSAController.FullyInitialized) {
                switch (onEvent) {
                    case InitializationEvent.OnLoadLocalData:
                        URSAController.OnLoadLocalData += Initialize;
                        break;
                    case InitializationEvent.OnSystemsEnabled:
                        URSAController.OnSystemsEnabled += Initialize;
                        break;
                    default:
                        break;
                }
            }
            else
                Initialize();
        }

        public override void OnDisable() {
            base.OnDisable();
            switch (onEvent) {
                case InitializationEvent.OnLoadLocalData:
                    URSAController.OnLoadLocalData -= Initialize;
                    break;
                case InitializationEvent.OnSystemsEnabled:
                    URSAController.OnSystemsEnabled -= Initialize;
                    break;
                default:
                    break;
            }
        }

        public virtual void Initialize() {

        }
    }
}