namespace URSA {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Enforces ordered execution of unity updates and onEnable through IOrderedBehavior interface
    /// Only works with direct children, intended for usage with URSA systems
    /// </summary>
    public class SystemsExecutor : MonoBehaviour {

        public static bool UpdateAllowed { get; set; }

        private List<ExecutionObject> objects = new List<ExecutionObject>();
        private List<Transform> transforms = new List<Transform>();
        void OnEnable() {
            objects.Clear();
            transforms.Clear();

            transforms = gameObject.GetTransformsInOrder();

            for (int i = 0; i < transforms.Count; i++) {
                objects.Add(
                    new ExecutionObject() {
                        gameObject = transforms[i].gameObject,
                        behavior = gameObject.GetComponent<SystemBase>()
                    });
            }
        }

        public void RunOrderedOnEnable() {

            int count = objects.Count;
            for (int i = 0; i < count; i++) {
                objects[i].gameObject.SetActive(true);
                if (objects[i].IsValidForOrderedCall) {
                    objects[i].behavior.OrderedOnEnable();
                }
            }
        }

        void FixedUpdate() {
            if (UpdateAllowed) {
                int count = objects.Count;
                for (int i = 0; i < count; i++) {
                    if (objects[i].IsValidForOrderedCall) {
                        objects[i].behavior.OrderedFixedUpdate();
                    }
                }
            }
        }

        void Update() {
            if (UpdateAllowed) {
                int count = objects.Count;
                for (int i = 0; i < count; i++) {
                    if (objects[i].IsValidForOrderedCall) {
                        objects[i].behavior.OrderedUpdate();
                    }
                }
            }
        }

        private class ExecutionObject {
            public bool IsValidForOrderedCall { get { return IsOrdered && IsActiveAndEnabled; } }
            public bool IsOrdered { get { return behavior != null; } }
            public bool IsActiveAndEnabled { get { return gameObject.activeSelf && behavior.enabled; } }
            public GameObject gameObject;
            public SystemBase behavior;
        }
    }
}