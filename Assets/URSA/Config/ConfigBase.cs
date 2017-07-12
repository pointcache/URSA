namespace URSA {
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;
    using URSA;


    public class ConfigBase : MonoBehaviour {

        public void OnEnable() {
            ConfigSystem.RegisterConfig(this);
        }

        public void OnDisable() {
            ConfigSystem.UnregisterConfig(this);
        }
    }


}