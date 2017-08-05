using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;
public class MatcherExampleSystem06 : ECSSystem {


    internal override void OrderedOnEnable() {
        base.OrderedOnEnable();


    }

    internal override void OrderedUpdate() {
        base.OrderedUpdate();

        foreach (var match in Matcher<ColorComponent06, RotatorComponent03>.All) {

            float sine = Mathf.Sin(Time.timeSinceLevelLoad);

            match.Entity.transform.localScale = Vector3.one * sine;

            match.ComponentA.color.r = sine;

            match.ComponentB.RotationSpeed = sine * 100;

            match.Entity.GetUnityComponent<MeshRenderer>().material.color = match.ComponentA.color;
        } 
    }

}
