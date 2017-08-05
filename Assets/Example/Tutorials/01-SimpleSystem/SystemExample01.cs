using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;

public class SystemExample01 : ECSSystem {



    /// <summary>
    /// Ordered means that it will be executed in order you put the systems in hierarchy under LocalSystems object.
    /// </summary>
    internal override void OrderedOnEnable() {
        base.OrderedOnEnable();

        Debug.Log("System 01 enabled");

    }

    internal override void OrderedUpdate() {
        base.OrderedUpdate();

        Debug.Log("System 01 update");

    }

    internal override void OrderedFixedUpdate() {
        base.OrderedFixedUpdate();

        Debug.Log("System 01 fixedUpdate");

    }

}
