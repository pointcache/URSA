namespace URSA.ECS {
    using UnityEngine;
    using URSA;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Shares the same mechanism for accessing entity as ComponentBase, but is not a component. Use
    /// when you need access to entity.
    /// </summary>
    public class ECSBehavior : MonoBehaviour {

        URSA.Entity entity;
        public URSA.Entity Entity
        {
            get {
                if ((object)entity == null)
                    entity = GetEntityRecursive(transform);
                return entity;
            }
        }
        URSA.Entity GetEntityRecursive(Transform tr) {
            if ((object)tr == null) {
                return null;
            }
            URSA.Entity ec = tr.gameObject.GetComponent<URSA.Entity>();
            if ((object)ec != null) {
                return ec;
            }
            else
                return GetEntityRecursive(tr.parent);
        }
    }

}