namespace URSA {
    using System.Collections;
    using UnityEngine;

    public class ECSComponent : MonoBehaviour {

        [NotEditableInt]
        public int componentID;

        private bool m_initialized;
        public bool Initialized { get { return m_initialized; } }

        

        public virtual void OnEnable() {
            ComponentPoolSystem.Register(this);
            if(Entity)
                Entity.Registry.RegisterECSComponent(this);
            if (!m_initialized) {
                Initialize();
                m_initialized = true;
            }
        }
        public virtual void OnDisable() {
            ComponentPoolSystem.Unregister(this);
            if(Entity)
                Entity.Registry.UnregisterECSComponent(this);
            entity = null;
        }

        Entity entity;
        public Entity Entity
        {
            get {
                if (((object)entity) == null)
                    entity = GetComponentInParent<Entity>();
                return entity;
            }
        }

        /// <summary>
        /// This methods will be called as soon as we complete deserialization of the component. 
        /// </summary>
        protected virtual void OnDeserialized() {

        }

        /// <summary>
        /// This method will be called the first time the Entity is created in the world. 
        /// Then it wont be called even if you save/load scene.
        /// Perform once in a lifetime operations here.
        /// </summary>
        protected virtual void Initialize() {

        }

        public T GetECSComponent<T>() where T : ECSComponent {
            return Entity.GetECSComponent<T>();
        }

        public void DisableOnEndOfFrame() {
            StartCoroutine(disableOnEndOfFrame());
        }

        IEnumerator disableOnEndOfFrame() {
            for (;;) {
                yield return new WaitForEndOfFrame();
                enabled = false;
            }
        }
    }


    public abstract class SerializedData {

    }

}