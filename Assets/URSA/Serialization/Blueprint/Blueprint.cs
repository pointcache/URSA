namespace URSA.Serialization.Blueprints {

    using System;
    using URSA.Serialization;

    [Serializable]
    public class Blueprint {

        public float GameVersion;
        public string Name;
        public SaveObject SaveObject;

    }
}