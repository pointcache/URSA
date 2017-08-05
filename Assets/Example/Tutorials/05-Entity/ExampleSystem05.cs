using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class ExampleSystem05 : ECSSystem {


	internal override void OrderedOnEnable() {
        base.OrderedOnEnable();


    }
	
	internal override void OrderedUpdate() {
        base.OrderedUpdate();

        //We iterate over each instance of this component in scene. Since we deliberately 
        //gave each entity only one of those as identifier we can use them to interate over Entities.
        //Imagine this being some component like - Building, Unit, Item and so on.
        foreach (var comp in Pool<RotatorComponent03>.Components) {

            //Every ECSComponent has a reference to its entity
            Entity entity = comp.Entity;

            //Iterating like this generates a bit of garbage
            foreach (var rotatorComponent in entity.AllOfType<RotatorComponent03>()) {

                rotatorComponent.transform.Rotate(Vector3.up, Time.deltaTime * rotatorComponent.RotationSpeed);

            }
            
            var subcomponents = entity.AllOfType<SubComponent05>();

            for (int i = 0; i < subcomponents.Count; i++) {

                SubComponent05 subcomp = subcomponents[i];

                if(Time.timeSinceLevelLoad > subcomp.LastCheckTime + 1f) {

                    subcomp.LastCheckTime = Time.timeSinceLevelLoad;

                    //We use unity's getcomponent as we know each of subcomponent has a Color component on the same game object.
                    var colorComp = subcomp.GetComponent<ColorComponent05>();

                    colorComp.color = UnityEngine.Random.ColorHSV();

                }
            }
        }
    }
}
