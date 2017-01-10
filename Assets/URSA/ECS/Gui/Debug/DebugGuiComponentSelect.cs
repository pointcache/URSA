using UnityEngine;
using System;
using System.Collections.Generic;

public class DebugGuiComponentSelect : MonoBehaviour {

    public ComponentBase comp;
	public void Select()
    {
        GameObject.FindObjectOfType<DebugGuiPropertyDrawer>().DrawComponent(comp);
    }
}
