using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class RotatorSystem : MonoBehaviour {

	void OrderedUpdate() {

        var list = Pool<Rotation>.components;

        foreach (var r in list) {

            r.currentRotation += Time.deltaTime * 10f;
            Vector3 rot = r.transform.rotation.eulerAngles;

            rot.x += r.currentRotation;
            r.transform.rotation = Quaternion.Euler(rot);
        }

    }
}
