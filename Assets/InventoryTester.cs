using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InventoryTester : MonoBehaviour {

    public ComponentBase weapon1;
    public Weapon toMakePersistent;

    private void OnEnable() {
        InitializerSystem.PostInitialization += init;
    }

    private void init() {
        weapon1 = Pool<Inventory>.First.data.weapon1;
    }

    public void MakePersistent() {
        toMakePersistent.Entity.MakePersistent();
    }
}
