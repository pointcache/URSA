using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;

public class DebugGuiContentPopulator : MonoBehaviour
{

    public GameObject component_button_prefab;
    RectTransform tr;
    private object iterations;

    private void OnEnable()
    {
        tr = GetComponent<RectTransform>();
    }
    public void PopulateWithConfigs()
    {
        tr.DestroyChildren();

        var components = GameObject.FindObjectsOfType<ComponentBase>();
        foreach (var c in components)
        {
            Type type = c.GetType();
            //var atts = type.GetCustomAttributes(false);
            if (!Attribute.IsDefined(type, typeof(ECSConfigAttribute)))
                continue;
            var go = GameObject.Instantiate(component_button_prefab);
            go.transform.SetParent(tr, false);
            Text t = go.GetComponentInChildren<Text>();
            t.text = type.Name;
            go.GetComponent<DebugGuiComponentSelect>().comp = c;
        }
    }
}
