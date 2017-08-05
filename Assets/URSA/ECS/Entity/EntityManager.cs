namespace URSA.ECS.Entity {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using URSA.Utility;
    using URSA.Serialization;
    using URSA;

    internal static class EntityManager {

        internal static Dictionary<int, URSA.Entity> SceneEntities = new Dictionary<int, URSA.Entity>(1000);
        internal static Dictionary<int, URSA.Entity> PersistentEntities = new Dictionary<int, URSA.Entity>(1000);

        internal static event Action<Entity> OnAdded;
        internal static event Action<Entity> OnRemoved;

        private static HashSet<int> registeredEntitiesIDs = new HashSet<int>();
        
        public static void MarkPersistent(Entity e) {
            SceneEntities.Remove(e.entityID);
            PersistentEntities.Add(e.entityID, e);
        }

        internal static void RegisterEntity(Entity e) {
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

            if (OnAdded != null)
                OnAdded(e);
        }

        internal static void UnRegisterEntity(Entity e) {
            SceneEntities.Remove(e.ID);
            PersistentEntities.Remove(e.ID);
            if (OnRemoved != null)
                OnRemoved(e);
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
            return component.Entity.GetECSComponent<T>();
        }
    }

}