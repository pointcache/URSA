#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

using UnityEditor;
using System.Collections;

public class MeshColliderReloader : MonoBehaviour {

    public void Reload() {
        StartCoroutine(ResetColliders(gameObject));
    }

    IEnumerator ResetColliders(GameObject g) {
        for (;;) {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            if (g) {
                var strs = g.GetComponentsInChildren<Transform>();
                foreach (Transform t in strs) {
                    var mc = t.GetComponent<MeshCollider>();
                    if (mc) {
                        mc.enabled = false;
                        mc.enabled = true;
                    }
                }
            }
            yield break;
        }
    }


}
#endif