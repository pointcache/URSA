namespace URSA.ECS.Entity {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using URSA.Utility;
    using URSA.Serialization;

    internal static class EntityManager {

        internal static Dictionary<int, URSA.Entity> SceneEntities = new Dictionary<int, URSA.Entity>(1000);
        internal static Dictionary<int, URSA.Entity> PersistentEntities = new Dictionary<int, URSA.Entity>(1000);

        private static HashSet<int> registeredEntitiesIDs = new HashSet<int>();
        
        internal static void RegisterEntity(URSA.Entity e) {
            var persistent_root = e.transform.GetComponentInParent<PersistentDataSystem>();
            if (persistent_root.Null()) {
                if (SceneEntities.ContainsKey(e.ID)) {
                    Debug.Log("Entity with this ID already exists, i will assume you duplicated it in the editor, so ill assign a new instance ID for you.");
                    e.instanceID = 0;
                }
                SceneEntities.Add(e.ID, e);

            }
            else {
                if (PersistentEntities.ContainsKey(e.ID)) {
                    Debug.Log("Entity with this ID already exists, i will assume you duplicated it in the editor, so ill assign a new instance ID for you.");
                    e.instanceID = 0;
                }
                PersistentEntities.Add(e.ID, e);
            }
        }

        internal static void UnRegisterEntity(URSA.Entity e) {
            SceneEntities.Remove(e.ID);
            PersistentEntities.Remove(e.ID);
        }

        internal static int GetUniqieInstanceID() {
            int id = registeredEntitiesIDs.Count + 1;
            if (registeredEntitiesIDs.Contains(id))
                id = GetUniqueIdRecursive(id);
            registeredEntitiesIDs.Add(id);
            return id;  
        }

        private static int GetUniqueIdRecursive(int previous) {
            int id = previous + 1;
            if (registeredEntitiesIDs.Contains(id))
                return GetUniqueIdRecursive(id);
            else
                return id;
        }

        internal static T GetEntityComponent<T>(this ECSComponent component) where T : ECSComponent {
            if ((object)component == null)
                return null;
            if ((object)component.Entity == null)
                return null;
            return component.Entity.GetEntityComponent<T>();
        }
    }

}