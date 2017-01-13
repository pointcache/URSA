using UnityEngine;
using System;
using System.Collections.Generic;

public static class EntityManager
{

    public static Dictionary<string, Entity> scene = new Dictionary<string, Entity>(1000);
    public static Dictionary<string, Entity> persistent = new Dictionary<string, Entity>(1000);

    public static void RegisterEntity(Entity e)
    {
        var persistent_root = e.transform.GetComponentInParents<PersistentDataSystem>();
        if (persistent_root.Null())
            scene.Add(e.ID, e);
        else
            persistent.Add(e.ID, e);
    }

    public static void UnRegisterEntity(Entity e)
    {
        scene.Remove(e.ID);
        persistent.Remove(e.ID);
    }

    public static string get_id()
    {
        Guid guid = Guid.NewGuid();
        string id = guid.ToString();
        if (scene.ContainsKey(id))
            return get_id();
        else
            return id;
    }

    public static T GetEntityComponent<T>(this ComponentBase component) where T : ComponentBase
    {
        if ((object)component == null)
            return null;
        if ((object)component.Entity == null)
            return null;
        return component.Entity.GetEntityComponent<T>();
    }
}
