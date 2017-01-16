using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class InitializationEventsReceiver : MonoBehaviour {

    public bool IncludeChildren;

    Action OnGlobalSystemsEnabled;
    Action OnLoadLocalData;
    Action OnLoadersEnabled;
    Action OnSystemsEnabled;
    Action OnUiEnabled;

    private void Awake() {

        if (!IncludeChildren) {

            OnGlobalSystemsEnabled = () => transform.SendMessage("OnGlobalSystemsEnabled", SendMessageOptions.DontRequireReceiver);
            OnLoadLocalData = () => transform.SendMessage("OnLoadLocalData", SendMessageOptions.DontRequireReceiver);
            OnLoadersEnabled = () => transform.SendMessage("OnLoadersEnabled", SendMessageOptions.DontRequireReceiver);
            OnSystemsEnabled = () => transform.SendMessage("OnSystemsEnabled", SendMessageOptions.DontRequireReceiver);
            OnUiEnabled = () => transform.SendMessage("OnUiEnabled", SendMessageOptions.DontRequireReceiver);
        } else {
            OnGlobalSystemsEnabled = () => transform.BroadcastMessage("OnGlobalSystemsEnabled", SendMessageOptions.DontRequireReceiver);
            OnLoadLocalData = () => transform.BroadcastMessage("OnLoadLocalData", SendMessageOptions.DontRequireReceiver);
            OnLoadersEnabled = () => transform.BroadcastMessage("OnLoadersEnabled", SendMessageOptions.DontRequireReceiver);
            OnSystemsEnabled = () => transform.BroadcastMessage("OnSystemsEnabled", SendMessageOptions.DontRequireReceiver);
            OnUiEnabled = () => transform.BroadcastMessage("OnUiEnabled", SendMessageOptions.DontRequireReceiver);
        }

        InitializerSystem.OnGlobalSystemsEnabled += OnGlobalSystemsEnabled;
        InitializerSystem.OnLoadLocalData += OnLoadLocalData;
        InitializerSystem.OnLoadersEnabled += OnLoadersEnabled;
        InitializerSystem.OnSystemsEnabled += OnSystemsEnabled;
        InitializerSystem.OnUiEnabled += OnUiEnabled;
    }

    private void OnDestroy() {
        InitializerSystem.OnGlobalSystemsEnabled -= OnGlobalSystemsEnabled;
        InitializerSystem.OnLoadLocalData -= OnLoadLocalData;
        InitializerSystem.OnLoadersEnabled -= OnLoadersEnabled;
        InitializerSystem.OnSystemsEnabled -= OnSystemsEnabled;
        InitializerSystem.OnUiEnabled -= OnUiEnabled;
    }
}
