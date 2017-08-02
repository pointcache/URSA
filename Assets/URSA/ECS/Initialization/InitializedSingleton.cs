
namespace URSA.ECS.Initialization {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using URSA.ECS.Initialization;

    public class InitializedSingleton<T> : InitializedBehavior where T : InitializedBehavior {

        private static T m_instance;
        public static T Instance
        {
            get {
                if (!m_instance)
                    m_instance = GameObject.FindObjectOfType<T>();
                return m_instance;
            }
        }
    }

}