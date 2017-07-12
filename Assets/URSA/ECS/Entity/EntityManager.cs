namespace URSA.ECS {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using URSA.Utility;
    using URSA.Serialization;

    internal static class EntityManager {

        internal static Dictionary<string, Entity> SceneEntities = new Dictionary<string, Entity>(1000);
        internal static Dictionary<string, Entity> PersistentEntities = new Dictionary<string, Entity>(1000);

        internal static void RegisterEntity(Entity e) {
            var persistent_root = e.transform.GetComponentInParent<PersistentDataSystem>();
            if (persistent_root.Null())
                SceneEntities.Add(e.ID, e);
            else
                PersistentEntities.Add(e.ID, e);
        }

        internal static void UnRegisterEntity(Entity e) {
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