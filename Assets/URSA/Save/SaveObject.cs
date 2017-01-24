namespace URSA.Save
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SaveObject
    {
        public bool isBlueprint;
        public List<EntityObject> entities = new List<EntityObject>();
        public List<CompRef> comprefs = new List<CompRef>();
        //public List<CompRef> comprefs = new List<CompRef>();
        /// <summary>
        /// entity id, (component persistent ID, component)
        /// </summary>
        public Dictionary<string, Dictionary<string, ComponentObject>> components = new Dictionary<string, Dictionary<string, ComponentObject>>();
    }
}