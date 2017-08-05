using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;

public class RotatorSystemExample02 : ECSSystem {

    internal override void OrderedUpdate() {
        base.OrderedUpdate();

        foreach (var component in Pool<RotatorComponent02>.Components) {

            component.transform.Rotate(Vector3.up, Time.deltaTime * 10);

        }
    }
}
