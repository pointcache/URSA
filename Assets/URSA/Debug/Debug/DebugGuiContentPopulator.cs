using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;
using URSA;
public class DebugGuiContentPopulator : MonoBehaviour
{

    const string component_button_prefab = "URSA/Debug/component";
    RectTransform tr;

    private void OnEnable()
    {
        tr = GetComponent<RectTransform>();
    }
    public void PopulateWithConfigs()
    {
        tr.DestroyChildren();

        var components = GameObject.FindObjectsOfType<ConfigBase>();
        foreach (var c in components)
        {
            Type type = c.GetType();
            //var atts = type.GetCustomAttributes(false);
            if (!Attribute.IsDefined(type, typeof(ConfigAttribute)))
                continue;
            var go = Helpers.Spawn(component_button_prefab);
            go.transform.SetParent(tr, false);
            Text t = go.GetComponentInChildren<Text>();
            t.text = type.Name;
            go.GetComponent<DebugGuiComponentSelect>().comp = c;
        }
    }
}
