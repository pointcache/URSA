namespace URSA {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using URSA.ECS;
    using URSA.Utility;

    using URSA.Serialization;
    using URSA.ECS.Entity;

    public class Entity : MonoBehaviour {

        [NotEditableInt]
        public int entityID;
        [NotEditableInt]
        public int instanceID;
        [NotEditableInt]
        public int blueprintID;


        public int ID
        {
            get {
                if (instanceID == 0) {
                    instanceID = EntityManager.GetUniqieInstanceID();
                }
                return instanceID;
            }
        }

#if UNITY_EDITOR
        private void Reset() {
            for (int i = 0; i < 50; i++) {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            }
        }
#endif
        private void OnEnable() {
            EntityManager.RegisterEntity(this);
            AddEntityReferenceToColliders();
        }

        private void OnDisable() {
            EntityManager.UnRegisterEntity(this);
        }

        public T GetEntityComponent<T>() where T : ECSComponent {
            T c = Pool<T>.GetComponent(ID);
            //HACK this shit is due to the fact that some components in entity will be enabled before others,
            //so on initialization you wont be able to get references to them through the pool, will fix later
            if (c.Null()) {
                c = GetComponentInChildren<T>(true);
            }
            return c;
        }

        public T GetEntityUnityComponent<T>() where T : Component {
            T c = null;
            c = GetComponentInChildren<T>(true);
            return c;
        }

        /// <summary>
        /// TODO optimize this shit
        /// Not the most performant way but will do for now.
        /// </summary>
        /// <returns></returns>
        public void GetAllEntityComponents(List<ECSComponent> comps) {

            comps.AddRange(GetComponents<ECSComponent>());
            foreach (Transform t in transform) {
                getAllCompsRecursive(t, comps);
            }
        }


        private void getAllCompsRecursive(Transform tr, List<ECSComponent> comps) {
            if (tr.GetComponent<Entity>())
                return;
            else {
                comps.AddRange(tr.GetComponents<ECSComponent>());
                foreach (Transform t in tr) {
                    getAllCompsRecursive(t, comps);
                }
            }
        }
        private void AddEntityReferenceToColliders() {

            Transform tr = transform;

            int count = tr.childCount;

            for (int i = 0; i < count; i++) {

                AddEntityReferenceToCollidersRecursive(tr.GetChild(i));

            }

        }

        private void AddEntityReferenceToCollidersRecursive(Transform tr) {

            int count = tr.childCount;

            for (int i = 0; i < count; i++) {

                AddEntityReferenceToCollidersRecursive(tr.GetChild(i));

            }

            if (tr.GetComponent<Collider>())
                tr.gameObject.AddComponent<EntityReference>();
        }

        public void MakePersistent() {
            PersistentDataSystem.MakePersistent(this);
        }
    }
}