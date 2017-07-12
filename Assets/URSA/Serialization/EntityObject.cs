namespace URSA.Serialization
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    [Serializable]
    public class EntityObject
    {
        public string database_ID;
        public string instance_ID;
        public string blueprint_ID;
        public string parentName;
        public string gameObjectName;
        public string prefabPath;
        public bool parentIsComponent;
        public bool parentIsEntity;
        public string parent_entity_ID;
        public string parent_component_ID;

        public Vector3 position;
        public Vector3 rotation;
    }
}