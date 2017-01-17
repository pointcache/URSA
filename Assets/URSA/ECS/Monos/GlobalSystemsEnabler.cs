using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


/// <summary>
/// Simply enables children one by one
/// </summary>
public class GlobalSystemsEnabler : MonoBehaviour {

    
	void OnGlobalSystemsEnabled() {
        foreach (Transform tr in transform) {
            tr.gameObject.SetActive(true);
        }
    }
}
