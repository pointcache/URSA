using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using URSA;

public class DebugGuiPropertyDrawer : MonoBehaviour
{
    public GameObject property_dummy_prefab;
    public GameObject property_generic_prefab;
    private RectTransform _tr;
    public RectTransform tr { get { if (!_tr) _tr = GetComponent<RectTransform>(); return _tr; } }
    SerializedData currentData;
    public void DrawComponent(ComponentBase comp) {
        if (comp.GetType().BaseType == typeof(ConfigBase))
            drawConfig(comp as ConfigBase);
        else
            drawComponent(comp);
    }
    
    void drawConfig(ConfigBase comp)
    {
        tr.DestroyChildren();
        Type type = comp.GetType();
        
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var f in fields)
        {
            string name = f.Name;
            Type ftype = f.FieldType;
            GameObject go = null;
            if (ftype == typeof(float) || ftype == typeof(int) || ftype == typeof(string))
            {
                go = GameObject.Instantiate(property_generic_prefab);
                go.GetComponent<DebugGuiEditableProperty>().Initialize(comp, f, null, null, name);
            }
            else
                if (ftype.BaseType.BaseType == typeof(rVar))
            {
                go = GameObject.Instantiate(property_generic_prefab);
                var rvar = f.GetValue(comp);
                var prop = rvar.GetType().GetProperty("Value");
                go.GetComponent<DebugGuiEditableProperty>().Initialize(comp, null, prop, rvar, name);
            }
            else
                continue;
            go.transform.SetParent(tr, false);
        }
    }

    void drawComponent(ComponentBase comp)
    {
        tr.DestroyChildren();
        Type type = comp.GetType();
        var data = type.GetField("data");
        var datatype = data.FieldType;
        currentData = data.GetValue(comp) as SerializedData;
        var fields = datatype.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var f in fields)
        {
            string name = f.Name;
            Type ftype = f.FieldType;
            GameObject go = null;
            if (ftype == typeof(float) || ftype == typeof(int) || ftype == typeof(string))
            {
                go = GameObject.Instantiate(property_generic_prefab);
                go.GetComponent<DebugGuiEditableProperty>().Initialize(currentData, f, null, null, name);
            }
            else
                if (ftype.BaseType == typeof(rVar))
            {
                go = GameObject.Instantiate(property_generic_prefab);
                var rvar = f.GetValue(currentData);
                var prop = rvar.GetType().GetProperty("value");
                go.GetComponent<DebugGuiEditableProperty>().Initialize(currentData, null, prop, rvar, name);
            }
            else
                continue;
            go.transform.SetParent(tr, false);
        }
    }
}
