namespace URSA {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class ECSSystem : MonoBehaviour {
        public bool DrawGizmos = true;

        internal virtual void OrderedOnEnable() { }
        internal virtual void OrderedFixedUpdate() { }
        internal virtual void OrderedUpdate() { }
        protected virtual void OnEnable() { }

        private void OnDrawGizmos() {
            if(DrawGizmos)
                SystemDrawGizmos();
        }


        protected virtual void SystemDrawGizmos() {

        }
    }

    public class ECSSystemSingleton<T> : ECSSystem where T : class {

        private static T m_instance;
        public static T Instance
        {
            get {
                if (m_instance == null)
                    m_instance = GameObject.FindObjectOfType(typeof(T)) as T;
                return m_instance;
            }
        }

    }
}