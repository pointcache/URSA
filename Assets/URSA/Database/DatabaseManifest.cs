namespace URSA.Database {

    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DatabaseManifest {

        public float GameVersion;
        public Dictionary<int, string> entity_id_adress = new Dictionary<int, string>();
        public Dictionary<string, int> entity_adress_id = new Dictionary<string, int>();

    }

}