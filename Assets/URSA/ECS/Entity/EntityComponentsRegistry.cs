namespace URSA.ECS {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using URSA;
    using System.Collections;


    /// <summary>
    /// This object speeds up component lookup by caching the components.
    /// This is optional, as other means of finding components exist,
    /// this however speeds up the process,
    /// and does no allocations after initialization.
    /// 
    /// The objects are pooled.
    /// </summary>
    public class EntityComponentsRegistry {

        private static List<EntityComponentsRegistry> pool = new List<EntityComponentsRegistry>(100);

        public static EntityComponentsRegistry GetFromPool() {

            int count = pool.Count;

            if (count == 0) {
                pool.Capacity += 100;
                for (int i = 0; i < 100; i++) {
                    pool.Add(new EntityComponentsRegistry());
                }
            }

            var ecr = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);

            foreach (var pair in ecr.components) {
                pair.Value.Clear();
            }

            return ecr;
        }

        private Dictionary<Type, IList> components = new Dictionary<Type, IList>(25);

        private List<ECSComponent> allComponents = new List<ECSComponent>(25);

        public List<T> AllOfType<T>() where T : ECSComponent {

            Type t = typeof(T);
            IList list;
            components.TryGetValue(t, out list);
            if(list == null) {
                list = MakeNewGenericList(t);
                components.Add(typeof(T), list);
            }
            return list as List<T>;
        }

        public List<ECSComponent> All
        {
            get { return allComponents; }
        }

        public void RegisterECSComponent(ECSComponent component) {

            Type t = component.GetType();

            IList comps;
            components.TryGetValue(t, out comps);

            if (comps == null) {
                comps = MakeNewGenericList(t);
                components.Add(t, comps);
            }

            comps.Add(component);
            allComponents.Add(component);

        }

        public IList MakeNewGenericList(Type t) {
            if (!typeof(ECSComponent).IsAssignableFrom(t)) {
                throw new Exception("Tried to create a lsit with non Component type");
            }
            Type listType = typeof(List<>);
            Type genericType = listType.MakeGenericType(t);
            IList list = (IList)Activator.CreateInstance(genericType);
            return list;
        }

        public void UnregisterECSComponent(ECSComponent component) {

            Type t = component.GetType();

            IList comps;
            components.TryGetValue(t, out comps);

            if (comps != null) {
                comps.Remove(component);
            }

            allComponents.Remove(component);

        }

        public void ReleaseToPool() {
            pool.Add(this);
        }

    }

}