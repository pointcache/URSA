using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;

public class RotatorSystemExample03 : ECSSystem {

    internal override void OrderedUpdate() {
        base.OrderedUpdate();

        foreach (var component in Pool<RotatorComponent03>.Components) {

            component.transform.Rotate(Vector3.up, Time.deltaTime * component.RotationSpeed);

        }
    }
}
