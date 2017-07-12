namespace URSA {

    using UnityEngine;
    using URSA.Config;

    public class ConfigBase : MonoBehaviour {

        public string FileName;

        public void OnEnable() {
            ConfigSystem.RegisterConfig(this);
        }

        public void OnDisable() {
            ConfigSystem.UnregisterConfig(this);
        }
    }


}