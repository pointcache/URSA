namespace URSA {

    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class ComponentPoolSystem : MonoBehaviour {

        #region SINGLETON
        private static ComponentPoolSystem _instance;
        public static ComponentPoolSystem instance
        {
            get {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<ComponentPoolSystem>();
                if (!_instance)
                    _instance = New();
                return _instance;
            }
        }
        #endregion

        public static ComponentPoolSystem New() {
            GameObject go = new GameObject("ComponentPool");
            var comp = go.AddComponent<ComponentPoolSystem>();
            GameObject.DontDestroyOnLoad(comp);
            return comp;
        }

        public static Dictionary<Type, Pool> pools_dict = new Dictionary<Type, Pool>(100);

        [Tooltip("Read only!")]
        public List<Pool> pools_view = new List<Pool>();

        public static ECSComponent GetFirstOf(Type type) {
            Pool pool;
            pools_dict.TryGetValue(type, out pool);
            if (pool == null)
                return null;
            return pool.GetFirst();
        }

        /// <summary>
        /// Register component in corresponding pool
        /// </summary>
        /// <param name="icomp"></param>
        public static void Register(ECSComponent comp) {
            Pool reg = null;
            Type t = comp.GetType();
            if (pools_dict.TryGetValue(t, out reg)) {
                reg.Register(comp);
            }
            else {
                reg = CreatePool(t);
                reg.Register(comp);
            }
        }

        public static void Unregister(ECSComponent comp) {
            Pool reg = null;
            Type t = comp.GetType();
            if (pools_dict.TryGetValue(t, out reg)) {
                reg.Unregister(comp);
            }
        }

        /// <summary>
        /// Simply creates a pool of type t.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Pool CreatePool(Type t) {
            if (!typeof(ECSComponent).IsAssignableFrom(t)) {
                throw new Exception("Tried to create a pool with non Component type");
            }
            Type elementType = Type.GetType(t.ToString());
            Type[] types = new Type[] { elementType };
            Type listType = typeof(Pool<>);
            Type genericType = listType.MakeGenericType(types);
            Pool register = (Pool)Activator.CreateInstance(genericType);
            pools_dict.Add(t, register);
            register.Name = t.Name;
            instance.pools_view.Add(register);
            return register;
        }

       // public static RVar GetRvarFrom(string componentType, string variableName) {
       //     var type = System.Type.GetType(componentType);
       //     if (type == null) {
       //         Debug.LogError("Cant find such component type.");
       //         return null;
       //     }
       //     var comp = ComponentPoolSystem.GetFirstOf(type);
       //     if (comp.GetType().IsSubclassOf(typeof(ConfigBase))) {
       //         return type.GetField(variableName).GetValue(comp) as RVar;
       //     }
       //     else {
       //         var data = type.GetField("data").GetValue(comp);
       //         return data.GetType().GetField(variableName).GetValue(data) as RVar;
       //     }
       //
       // }
    }

    [Serializable]
    public class Pool {
        public string Name;
        public int count;

        public virtual void Register(ECSComponent comp) { }

        public virtual void Unregister(ECSComponent comp) { }

        public virtual ECSComponent GetFirst() { return null; }
    }

    /// <summary>
    /// A concrete generic implementation of the pool, mixes static components with dynamic
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> : Pool where T : ECSComponent {
        /// <summary>
        /// all components
        /// </summary>
        public static List<T> Components = new List<T>(1000);

        static Dictionary<int, List<T>> entities = new Dictionary<int, List<T>>(1000);

        private static T first;
        public static T First
        {
            get {
                return first;
            }
        }

        public static T FirstOrFindInScene
        {
            get {
                T f = first;
                if (!f)
                    f = GameObject.FindObjectOfType(typeof(T)) as T;
                return f;
            }
        }

        public override ECSComponent GetFirst() {
            return First;
        }

        public static event Action<T> OnAdded = delegate { };

        public static event Action<T> OnRemoved = delegate { };

        public static void RunMethodOnEach(Action<T> method) {
            int count = Count;
            for (int i = count - 1; i > -1 ; i--) {
                method(Components[i]);
            }
        }

        public static void RunMethodOnEachAndSubscribe(Action<T> method) {
            RunMethodOnEach(method);
            OnAdded += method;
        }

        public static bool Empty
        {
            get {
                return Components.Count == 0 ? true : false;
            }
        }

        public static int Count { get { return Components.Count; } }

        public static T Random
        {
            get {
                if (Count > 1) {
                    return Components[UnityEngine.Random.Range(0, Count)];
                }
                else {
                    return First;
                }
            }
        }

        public static T GetComponent(int entityID) {
            List<T> list = null;
            entities.TryGetValue(entityID, out list);

            if (list == null || list.Count == 0)
                return null;
            return list[0];
        }

        public override void Register(ECSComponent comp) {
            Components.Add(comp as T);
            count++;

            //if entity exists
            URSA.Entity e = comp.Entity;
            if ((object)e != null) {
                List<T> list = null;
                entities.TryGetValue(e.ID, out list);
                if (list == null) {
                    list = new List<T>(100);
                    entities.Add(e.ID, list);
                }
                list.Add(comp as T);    
            }

            if (OnAdded.GetInvocationList().Length > 1)
                OnAdded(comp as T);

            if (Components != null && Components.Count != 0)
                first = Components[0];
            else
                first = null;
        }

        public override void Unregister(ECSComponent comp) {
            Components.Remove(comp as T);
            count--;

            URSA.Entity e = comp.Entity;
            if ((object)e != null) {
                List<T> list = null;
                entities.TryGetValue(e.ID, out list);
                if (list != null) {
                    list.Remove(comp as T);
                }
            }

            if (OnRemoved.GetInvocationList().Length > 1)
                OnRemoved(comp as T);

            if (Components != null && Components.Count > 0)
                first = Components[0];
            else
                first = null;
        }

        public static void DestroyAll() {
            while (Count > 0)
                GameObject.Destroy(First);
        }
    }
}