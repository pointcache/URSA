using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;
using URSA.Utility;

public class PlayerProfileSystem13 : ECSSystem {

    [SerializeField]
    private GameObject profilePrefab;

    internal override void OrderedOnEnable() {
        base.OrderedOnEnable();

        var profile = GameObjectUtils.Spawn(profilePrefab, false);
        profile.GetEntity().MakePersistent();
    }
}
