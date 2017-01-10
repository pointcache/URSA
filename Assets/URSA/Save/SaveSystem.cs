using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA.Save;
public class SaveSystem : MonoBehaviour {

    public string savePath = "";

    //creates a file containing all entities in scene, ignoring any scene specifics
    public void SaveScene()
    {
        SaveFile file = new SaveFile();

        foreach (var pair in EntityManager.all)
        {
            EntityObject eobj = new EntityObject();
            Entity entity = pair.Value;
            eobj.database_ID = entity.database_ID;
            eobj.instance_ID = entity.instance_ID;
            eobj.position = entity.transform.position;
            eobj.rotation = entity.transform.rotation.eulerAngles;
            eobj.scale = entity.transform.localScale;

            file.entities.Add(eobj);

            var components = entity.GetComponentsInChildren<ComponentBase>(true);

            foreach (var comp in components)
            {
                
            }
        }
    }
}
