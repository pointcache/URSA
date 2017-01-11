using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PersistentDataSystem : MonoBehaviour {


    #region SINGLETON
    private static PersistentDataSystem _instance;
    public static PersistentDataSystem instance
    {
        get {
            if (!_instance) _instance = GameObject.FindObjectOfType<PersistentDataSystem>();
            return _instance;
        }
    }
    #endregion

    public static void MakePersistent (Entity entity) {
        UnityEngine.Object.DontDestroyOnLoad(entity.gameObject);
        entity.transform.parent = instance.transform;
    }
}
