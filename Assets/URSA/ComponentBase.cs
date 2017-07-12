namespace URSA {

    using UnityEngine;
    using URSA.Utility.NotEditableString;
    using URSA.ECS.Components;

    public class ComponentBase : MonoBehaviour {
        [NotEditableString]
        public string ID;

        public virtual void OnEnable() {
            ComponentPoolSystem.Register(this);
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

   }
    public abstract class SerializedData { }
}