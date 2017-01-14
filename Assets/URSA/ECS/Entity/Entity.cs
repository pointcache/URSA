using UnityEngine;
using System;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
  
    [NotEditableString]
    public string database_ID;
    [NotEditableString]
    public string instance_ID;
    
    [NotEditableString]
    public string blueprint_ID;

    public string ID
    {
        get
        {
            if (String.IsNullOrEmpty(instance_ID))
            {
                instance_ID = EntityManager.get_id();
            }
            return instance_ID;
        }
    }

    void OnEnable()
    {
        EntityManager.RegisterEntity(this);
    }

    void OnDisable()
    {
        EntityManager.UnRegisterEntity(this);
    }

    public T GetEntityComponent<T>() where T : ComponentBase
    {
        return Pool<T>.getComponent(ID);
    }
    
    /// <summary>
    /// Not the most performant way but will do for now.
    /// </summary>
    /// <returns></returns>
    public List<ComponentBase> GetAllEntityComponents() {

        List<ComponentBase> comps = new List<ComponentBase>();
        comps.AddRange(GetComponents<ComponentBase>());
        getAllCompsRecursive(transform, comps);

        foreach (Transform t in transform) {
            getAllCompsRecursive(t, comps);
        }
        return comps;
    }

    void getAllCompsRecursive(Transform tr, List<ComponentBase> comps) {
        if (tr.GetComponent<Entity>())
            return;
        else {
            comps.AddRange(tr.GetComponents<ComponentBase>());
            foreach (Transform t in tr) {
                getAllCompsRecursive(t, comps);
            }
        }
    }

    public void MakePersistent() {
        PersistentDataSystem.MakePersistent(this);
    }

}
