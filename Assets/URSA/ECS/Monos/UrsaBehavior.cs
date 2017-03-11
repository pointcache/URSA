using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// Shares the same mechanism for accessing entity as ComponentBase, but is not a component. Use
/// when you need access to entity.
/// </summary>
public class UrsaBehavior : MonoBehaviour {

    Entity _entity;
    public Entity entity
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
