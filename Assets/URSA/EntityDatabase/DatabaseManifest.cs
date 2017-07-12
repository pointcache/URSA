namespace URSA.EntityDatabase {

    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DatabaseManifest {

        public float GameVersion;
        public Dictionary<string, string> entity_id_adress = new Dictionary<string, string>();
        public Dictionary<string, string> entity_adress_id = new Dictionary<string, string>();

    }

}