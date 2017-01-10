using UnityEngine;
using System;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
    [NotEditableString]
    public string database_ID;
    private string _ID;
    public string ID
    {
        get
        {
            if (String.IsNullOrEmpty(_ID))
            {
                _ID = EntityManager.get_id();
            }
            return _ID;
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

    //Dictionary<Type, List<ComponentBase>> components = new Dictionary<Type, List<ComponentBase>>();
    //
    //void OnEnable()
    //{
    //    CollectEntities();
    //}
    //
    //void CollectEntities()
    //{
    //    var comps = GetComponentsInChildren(typeof(ComponentBase));
    //
    //    int count = comps.Length;
    //
    //    for (int i = 0; i < count; i++)
    //    {
    //        Type type = comps[i].GetType();
    //        List<ComponentBase> list = null;
    //        components.TryGetValue(type, out list);
    //        if (list == null)
    //        {
    //            list = new List<ComponentBase>(10);
    //        }
    //
    //        list.Add(w)
    //        components.Add(type, list);
    //    }
    //
    //}
}
