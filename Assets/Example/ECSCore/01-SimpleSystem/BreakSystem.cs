using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;

public class BreakSystem : ECSSystem {


    internal override void OrderedUpdate() {
        base.OrderedUpdate();
        Debug.Log("BreakSystem did Debug.Break()");
        Debug.Break();

    }

}
