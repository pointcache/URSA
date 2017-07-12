namespace URSA {
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Shares the same mechanism for accessing entity as ComponentBase, but is not a component. Use
    /// when you need access to entity.
    /// </summary>
    public class UrsaBehavior : MonoBehaviour {

        Entity entity;
        public Entity Entity
        {
            get {
                if ((object)entity == null)
                    entity = GetEntityRecursive(transform);
                return entity;
            }
        }
        Entity GetEntityRecursive(Transform tr) {
            if ((object)tr == null) {
                return null;
            }
            Entity ec = tr.gameObject.GetComponent<Entity>();
            if ((object)ec != null) {
                return ec;
            }
            else
                return GetEntityRecursive(tr.parent);
        }
    }

}