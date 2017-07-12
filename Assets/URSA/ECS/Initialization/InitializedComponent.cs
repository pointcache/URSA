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
            if (!ECSController.FullyInitialized) {
                switch (onEvent) {
                    case InitializationEvent.OnLoadLocalData:
                        ECSController.OnLoadLocalData += Initialize;
                        break;
                    case InitializationEvent.OnSystemsEnabled:
                        ECSController.OnSystemsEnabled += Initialize;
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
                    ECSController.OnLoadLocalData -= Initialize;
                    break;
                case InitializationEvent.OnSystemsEnabled:
                    ECSController.OnSystemsEnabled -= Initialize;
                    break;
                default:
                    break;
            }
        }

        public virtual void Initialize() {

        }
    }
}