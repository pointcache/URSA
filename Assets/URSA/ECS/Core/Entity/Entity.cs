using UnityEngine;
using System;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
  
    [NotEditableString]
    public string database_ID;
    [NotEditableString]
    public string instance_ID;

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

    public void MakePersistent() {
        PersistentDataSystem.MakePersistent(this);
    }

}
