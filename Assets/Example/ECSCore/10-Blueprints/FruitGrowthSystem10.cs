using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class FruitGrowthSystem10 : ECSSystem {


    internal override void OrderedOnEnable() {
        base.OrderedOnEnable();


    }

    internal override void OrderedUpdate() {
        base.OrderedUpdate();

        var fruits = Pool<FruitComponent10>.Components;

        foreach (var fruit in fruits) {
            if (fruit.data.size < 1f) {
                fruit.data.size += fruit.data.growthRate * Time.deltaTime;
                fruit.Entity.transform.localScale = Vector3.one * fruit.data.size;
            }
            else {
                fruit.Entity.gameObject.AddComponent<Rigidbody>();
                fruit.DisableOnEndOfFrame();
            }
        }
    }

}
