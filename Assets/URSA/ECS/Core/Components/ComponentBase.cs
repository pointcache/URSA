using UnityEngine;
using System;
using System.Collections.Generic;


public class ComponentBase : MonoBehaviour
{
    [NotEditableString]
    public string ID;

    public virtual void OnEnable()
    {
        ComponentPoolSystem.Register(this);
    }
    public virtual void OnDisable()
    {
        ComponentPoolSystem.Unregister(this);
        _entity = null;
    }

    public virtual SerializedData GetData()
    {
        return null;
    }

    public virtual void SetData(object _data)
    {
    }

    Entity _entity;
    public Entity Entity
    {
        get
        {
            if ((object)_entity == null)
                _entity = getEntityRecursive(transform);
            return _entity;
        }
    }
    Entity getEntityRecursive(Transform tr)
    {
        if ((object)tr == null)
        {
            return null;
        }
        Entity ec = tr.gameObject.GetComponent<Entity>();
        if ((object)ec != null)
        {
            return ec;
        }
        else
            return getEntityRecursive(tr.parent);
    }
}
public class SerializedData { }
public class ECSConfigAttribute : Attribute
{
}