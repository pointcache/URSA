using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class performancetest01system : ECSSystem {


	internal override void OrderedOnEnable() {
        base.OrderedOnEnable();

        var entity = Pool<RotatorComponent03>.First.Entity;

        for (int i = 0; i < 2000; i++) {
            Instantiate(entity.gameObject);
        }
    }
	
	internal override void OrderedUpdate() {
        base.OrderedUpdate();

        var comps = Pool<RotatorComponent03>.Components;

        int count = comps.Count;

        for (int i = 0; i < count; i++) {

            var comp = comps[i];

            //Every ECSComponent has a reference to its entity
            Entity entity = comp.Entity;

            var rotators = entity.AllOfType<RotatorComponent03>();
            int rotatorsCount = rotators.Count;

            for (int z = 0; z < rotatorsCount; z++) {
                rotators[z].transform.Rotate(Vector3.up, Time.deltaTime * rotators[z].RotationSpeed);
            }

            var subcomponents = entity.AllOfType<SubComponent05>();

            for (int t = 0; t < subcomponents.Count; t++) {

                SubComponent05 subcomp = subcomponents[t];

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
