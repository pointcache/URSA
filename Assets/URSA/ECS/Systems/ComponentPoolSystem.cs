using UnityEngine;
using System;
using System.Collections.Generic;

public class ComponentPoolSystem : MonoBehaviour
{

    #region SINGLETON
    private static ComponentPoolSystem _instance;
    public static ComponentPoolSystem instance
    {
        get
        {
            if (!_instance)
                _instance = GameObject.FindObjectOfType<ComponentPoolSystem>();
            if (!_instance)
                _instance = New();
            return _instance;
        }
    }
    #endregion

    public static ComponentPoolSystem New()
    {
        GameObject go = new GameObject("ComponentPool");
        var comp = go.AddComponent<ComponentPoolSystem>();
        return comp;
    }

    public static Dictionary<Type, Pool> registers = new Dictionary<Type, Pool>(100);

    [Tooltip("Read only!")]
    public List<Pool> registers_view = new List<Pool>();

    /// <summary>
    /// Register component in corresponding pool
    /// </summary>
    /// <param name="icomp"></param>
    public static void Register(ComponentBase comp)
    {
        Pool reg = null;
        Type t = comp.GetType();
        if (registers.TryGetValue(t, out reg))
        {
            reg.register(comp);
        }
        else
        {
            reg = CreatePool(t);
            reg.register(comp);
        }
    }

    public static void Unregister(ComponentBase comp)
    {
        Pool reg = null;
        Type t = comp.GetType();
        if (registers.TryGetValue(t, out reg))
        {
            reg.unregister(comp);
        }
    }

    /// <summary>
    /// Simply creates a pool of type t.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Pool CreatePool(Type t)
    {
        if (!typeof(ComponentBase).IsAssignableFrom(t))
        {
            throw new Exception("Tried to create a pool with non Component type");
        }
        Type elementType = Type.GetType(t.ToString());
        Type[] types = new Type[] { elementType };
        Type listType = typeof(Pool<>);
        Type genericType = listType.MakeGenericType(types);
        Pool register = (Pool)Activator.CreateInstance(genericType);
        //((IPoolAccessor)pool).Initialize();
        registers.Add(t, register);
        register.Name = t.Name;
        instance.registers_view.Add(register);
        return register;
    }
}

[Serializable]
public class Pool
{
    public string Name;
    public int count;

   // public List<Component> comps = new List<Component>();

    public virtual void register(ComponentBase comp)
    { }

    public virtual void unregister(ComponentBase comp)
    { }
}

/// <summary>
/// A concrete generic implementation of the pool, mixes static components with dynamic
/// </summary>
/// <typeparam name="T"></typeparam>
public class Pool<T> : Pool where T : ComponentBase
{
    /// <summary>
    /// all components
    /// </summary>
    public static List<T> components = new List<T>(1000);
    static Dictionary<string, List<T>> entities = new Dictionary<string, List<T>>(1000);
    public static T First
    {
        get
        {
            if (components == null || components.Count == 0)
                return null;
            return components[0];
        }
    }

    public static event Action<T> OnAdded = delegate { };
    public static event Action<T> OnRemoved = delegate { };
    public static bool Empty
    {
        get
        {
            return components.Count == 0 ? true : false;
        }
    }
    public static int Count { get { return components.Count; } }

    public static T getComponent(string entityID)
    {
        List<T> list = null;
        entities.TryGetValue(entityID, out list);

        if (list == null)
            return null;
        if (list.Count == 0)
            return null;
        return list[0];
    }

    public override void register(ComponentBase comp)
    {
        components.Add(comp as T);
        count++;

        //if entity exists
        Entity e = comp.Entity;
        if((object)e != null)
        {
            List<T> list = null;
            entities.TryGetValue(e.ID, out list);
            if (list == null)
            {
                list = new List<T>(100);
                entities.Add(e.ID, list);
            }
            list.Add(comp as T);
        }

        if(OnAdded.GetInvocationList().Length > 1)
            OnAdded(comp as T);
    }

    public override void unregister(ComponentBase comp)
    {
        components.Remove(comp as T);
        count--;

        Entity e = comp.Entity;
        if((object)e != null)
        {
            List<T> list = null;
            entities.TryGetValue(e.ID, out list);
            if (list != null)
            {
                list.Remove(comp as T);
            }
        }

        if (OnRemoved.GetInvocationList().Length > 1)
            OnRemoved(comp as T);
    }
}