using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;


/// <summary>
/// This is a singleton system, you can access it through Instance property
/// </summary>
public class SystemExample02_Singleton : ECSSystemSingleton<SystemExample02_Singleton> {



    /// <summary>
    /// Ordered means that it will be executed in order you put the systems in hierarchy under LocalSystems object.
    /// </summary>
    internal override void OrderedOnEnable() {
        base.OrderedOnEnable();

        Debug.Log("System 02 enabled");

    }

    internal override void OrderedUpdate() {
        base.OrderedUpdate();

        Debug.Log("System 02 update");

    }

    internal override void OrderedFixedUpdate() {
        base.OrderedFixedUpdate();

        Debug.Log("System 02 fixedUpdate");

    }

}
