namespace URSA {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EntityReference : MonoBehaviour {

        Entity entity;
        public Entity Entity
        {
            get {
                if (((object)entity) == null)
                    entity = GetComponentInParent<Entity>();
                return entity;
            }
        }
    }

}