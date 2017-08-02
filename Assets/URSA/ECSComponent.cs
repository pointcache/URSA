namespace URSA {

    using UnityEngine;

    public class ECSComponent : MonoBehaviour {

        [NotEditableInt]
        public int componentID;

        private bool m_initialized;
        public bool Initialized { get { return m_initialized; } }

        

        public virtual void OnEnable() {
            ComponentPoolSystem.Register(this);
            if (!m_initialized) {
                Initialize();
                m_initialized = true;
            }
        }
        public virtual void OnDisable() {
            ComponentPoolSystem.Unregister(this);
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
        /// This method will be called the first time the Entity is created in the world. 
        /// Then it wont be called even if you save/load scene.
        /// Perform once in a lifetime operations here.
        /// </summary>
        public virtual void Initialize() {

        }

        public T GetEntityComponent<T>() where T : ECSComponent {
            return Entity.GetEntityComponent<T>();
        }

    }

    public abstract class SerializedData {

    }

}