using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;
using URSA.ECS.Entity;
using URSA.Utility;

public static class Matcher<T, T1> where T : ECSComponent where T1 : ECSComponent {

    private static List<MatchedComponents> pool = new List<MatchedComponents>(100);

    private static MatchedComponents GetNew(T comp, T1 comp1, Entity e) {
        int count = pool.Count;

        if (pool.Count == 0) {
            var m = new MatchedComponents();
            m.SetValues(comp, comp1, e);
            return m;
        }

        else {
            var m = pool[count-1];
            pool.RemoveAt(count-1);
            return m;
        }

    }

    public class MatchedComponents {

        private T componentA;
        public T ComponentA
        {
            get { return componentA; }
        }

        private T1 componentB;
        public T1 ComponentB
        {
            get { return componentB; }
        }

        private Entity entity;
        public Entity Entity
        {
            get { return entity; }
        }

        public void SetValues(T comp, T1 comp1, Entity e) {
            componentA = comp;
            componentB = comp1;
            entity = e;
        }
    }

    private static List<MatchedComponents> matched = new List<MatchedComponents>();
    public static List<MatchedComponents> All
    {
        get {
            return matched;
        }
    }

    private static Dictionary<Entity, MatchedComponents> entity_matched_dict = new Dictionary<Entity, MatchedComponents>();

    static Matcher() {

        EntityManager.OnAdded += ProcessAdded;

        foreach (var pair in EntityManager.SceneEntities) {
            ProcessAdded(pair.Value);
        }
        
        pool = CollectionUtilities.GetPopulatedListRefType<MatchedComponents>(100);

    }

    static void ProcessAdded(Entity e) {

        T comp = e.GetECSComponent<T>();
        if (comp.Null())
            return;

        T1 comp1 = e.GetECSComponent<T1>();
        if (comp1.Null())
            return;

        Add(comp.Entity, comp, comp1);

    }

    static void Add(Entity e, T comp, T1 comp1) {

        MatchedComponents m = GetNew(comp, comp1, e);
        matched.Add(m);
        entity_matched_dict.Add(e, m);

    }

    static void ProcessRemoved(Entity e) {

        MatchedComponents m;

        entity_matched_dict.TryGetValue(e, out m);

        if (m != null) {
            matched.Remove(m);
            pool.Add(m);
            entity_matched_dict.Remove(e);
        }

    }
}
