namespace URSA.Save
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SaveFile
    {
        public List<EntityObject> entities = new List<EntityObject>();
        /// <summary>
        /// entity id, (component persistent ID, component)
        /// </summary>
        public Dictionary<string, Dictionary<string, ComponentObject>> components = new Dictionary<string, Dictionary<string, ComponentObject>>();
    }
}