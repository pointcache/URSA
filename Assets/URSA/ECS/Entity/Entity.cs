namespace URSA {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using URSA.ECS;
    using URSA.Utility;
    using URSA.Utility.NotEditableString;
    using URSA.Serialization;

    public class Entity : MonoBehaviour {

        [NotEditableString]
        public string database_ID;
        [NotEditableString]
        public string instance_ID;

        [NotEditableString]
        public string blueprint_ID;
        public string ID
        {
            get {
                if (String.IsNullOrEmpty(instance_ID)) {
                    instance_ID = EntityManager.GetUniqieID();
                }
                return instance_ID;
            }
        }
#if UNITY_EDITOR
        private void Reset() {
            for (int i = 0; i < 50; i++) {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            }
        }
#endif
        void OnEnable() {
            EntityManager.RegisterEntity(this);
        }
        void OnDisable() {
            EntityManager.UnRegisterEntity(this);
        }
        public T GetEntityComponent<T>() where T : ComponentBase {
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
        /// Not the most performant way but will do for now.
        /// </summary>
        /// <returns></returns>
        public List<ComponentBase> GetAllEntityComponents() {
            List<ComponentBase> comps = new List<ComponentBase>();
            comps.AddRange(GetComponents<ComponentBase>());
            getAllCompsRecursive(transform, comps);
            foreach (Transform t in transform) {
                getAllCompsRecursive(t, comps);
            }
            return comps;
        }
        void getAllCompsRecursive(Transform tr, List<ComponentBase> comps) {
            if (tr.GetComponent<Entity>())
                return;
            else {
                comps.AddRange(tr.GetComponents<ComponentBase>());
                foreach (Transform t in tr) {
                    getAllCompsRecursive(t, comps);
                }
            }
        }
        public void MakePersistent() {
            PersistentDataSystem.MakePersistent(this);
        }
    }


}