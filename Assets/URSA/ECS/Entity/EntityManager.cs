namespace URSA.ECS.Entity {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using URSA.Utility;
    using URSA.Serialization;

    internal static class EntityManager {

        internal static Dictionary<string, URSA.Entity> SceneEntities = new Dictionary<string, URSA.Entity>(1000);
        internal static Dictionary<string, URSA.Entity> PersistentEntities = new Dictionary<string, URSA.Entity>(1000);

        internal static void RegisterEntity(URSA.Entity e) {
            var persistent_root = e.transform.GetComponentInParent<PersistentDataSystem>();
            if (persistent_root.Null())
                SceneEntities.Add(e.ID, e);
            else
                PersistentEntities.Add(e.ID, e);
        }

        internal static void UnRegisterEntity(URSA.Entity e) {
            SceneEntities.Remove(e.ID);
            PersistentEntities.Remove(e.ID);
        }

        internal static string GetUniqieID() {
            Guid guid = Guid.NewGuid();
            string id = guid.ToString();
            if (SceneEntities.ContainsKey(id))
                return GetUniqieID();
            else
                return id;
        }

        internal static T GetEntityComponent<T>(this ComponentBase component) where T : ComponentBase {
            if ((object)component == null)
                return null;
            if ((object)component.Entity == null)
                return null;
            return component.Entity.GetEntityComponent<T>();
        }
    }

}