using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GlobalSystemsLoader : MonoBehaviour {


    #region SINGLETON
    private static GlobalSystemsLoader _instance;
    public static GlobalSystemsLoader instance
    {
        get
        {
            if (!_instance) _instance = GameObject.FindObjectOfType<GlobalSystemsLoader>();
            return _instance;
        }
    }
    #endregion



    public static GlobalSystemsLoader current;
    GameObject initializer;

    void OnEnable()
    {
        if (current)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            current = this;

            initializer = transform.Find("Initializer").gameObject;
            if(!initializer)
            {
                Debug.LogError("GlobalSystems: No Systems gameobject found");
                return;
            }
            initializer.SetActive(true);
            DontDestroyOnLoad(gameObject);
        }
    }
}
