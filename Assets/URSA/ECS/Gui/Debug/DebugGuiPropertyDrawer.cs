using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
public class DebugGuiPropertyDrawer : MonoBehaviour
{
    public GameObject property_dummy_prefab;
    public GameObject property_generic_prefab;
    private RectTransform _tr;
    public RectTransform tr { get { if (!_tr) _tr = GetComponent<RectTransform>(); return _tr; } }
    ComponentData currentData;

    public void DrawComponent(ComponentBase comp)
    {
        tr.DestroyChildren();
        Type type = comp.GetType();
        var data = type.GetField("data");
        var datatype = data.FieldType;
        currentData = data.GetValue(comp) as ComponentData;
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
