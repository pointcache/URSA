namespace URSA.Serialization
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    [Serializable]
    public class EntityObject
    {
        public int database_ID;
        public int instance_ID;
        public int blueprint_ID;
        public string parentName;
        public string gameObjectName;
        public string prefabPath;
        public bool parentIsComponent;
        public bool parentIsEntity;
        public int parent_entity_ID;
        public int parent_component_ID;

        public Vector3 position;
        public Vector3 rotation;
    }
}