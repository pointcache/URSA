using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GlobalSystemsEnabler : MonoBehaviour {

	void OnGlobalSystemsEnabled() {
        foreach (Transform tr in transform) {
            tr.gameObject.SetActive(true);
        }
    }
}
