namespace URSA.Blueprint {

    using System;
    using URSA.Save;

    [Serializable]
    public class Blueprint {

        public float GameVersion;
        public string Name;
        public SaveObject SaveObject;

    }
}