namespace URSA {

    using UnityEngine;
    using URSA.Config;

    public class ConfigBase : MonoBehaviour {

        public string FileName;

        protected virtual void OnEnable() {
            ConfigSystem.RegisterConfig(this);
        }

        protected virtual void OnDisable() {
            ConfigSystem.UnregisterConfig(this);
        }
    }


}