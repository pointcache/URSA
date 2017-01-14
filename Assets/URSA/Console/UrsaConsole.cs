namespace URSA {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Reflection;
    using SmartConsole;

    public class UrsaConsole : MonoBehaviour {

        #region SINGLETON
        private static UrsaConsole _instance;
        public static UrsaConsole instance
        {
            get {
                if (!_instance) _instance = GameObject.FindObjectOfType<UrsaConsole>();
                return _instance;
            }
        }
        #endregion

        [Header("Console")]
        public SmartConsole.Options consoleOptions;

        static HashSet<string> uniqueNameCheck = new HashSet<string>();

        GameObject consoleRoot;
        void OnEnable() {
            var evsys = GameObject.FindObjectOfType<EventSystem>();
            if (!evsys) {
                Debug.LogError("UnityEvent System not found in scene, manually add it.");
                Debug.Break();
            }
            GameObject prefab = Resources.Load<GameObject>("BeastConsole/BeastConsole");
            consoleRoot = GameObject.Instantiate(prefab);
            consoleRoot.transform.SetParent(transform);
            SmartConsole.options = consoleOptions;
            SmartConsole.entryTemplate = Resources.Load<GameObject>("BeastConsole/ConsoleEntry");
            SmartConsole.consoleContent = consoleRoot.transform.FindDeepChild("Content").gameObject;
            SmartConsole.consoleRoot = consoleRoot.transform.FindDeepChild("Root").GetComponent<RectTransform>();
            SmartConsole.inputField = consoleRoot.transform.FindDeepChild("InputField").GetComponent<InputField>();
            SmartConsole.scrollBar = consoleRoot.transform.FindDeepChild("Scrollbar Vertical").GetComponent<Scrollbar>();
            consoleRoot.AddComponent<SmartConsole>();

            RegisterCommandWithParameters("spawn", Spawn);
        }

        private void OnDisable() {
            SmartConsole.Destroy();
            Destroy(consoleRoot.gameObject);
        }

        public static void RegisterVar(IrVar irvar, FieldInfo f) {
            var attr = f.GetCustomAttributes(typeof(ConsoleVarAttribute), false);
            if (attr.Length == 1) {
                ConsoleVarAttribute cattr = attr[0] as ConsoleVarAttribute;

                if (uniqueNameCheck.Contains(cattr.Name)) {
                    Debug.LogError("<color=red> You can't have config variables with same name. </color>");
                    Debug.LogError("Variable with name : " + cattr.Name);
                    return;
                }
                irvar.registerInConsole(cattr.Name, cattr.Description);
            }
        }

        public static void WriteLine(string line) {
            SmartConsole.WriteLine(line);
        }
        
        /// <summary>
        /// Provide parameters as string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        public static void RegisterCommand(string name, Action command) {
            SmartConsole.RegisterCommand(name, "no description", "no example", command);
        }

        /// <summary>
        /// Provide parameters as string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        public static void RegisterCommandWithParameters(string name, Action<string[]> command) {
            SmartConsole.RegisterCommandWithParameters(name, "no description", "no example", command);
        }

        public static void Spawn(string[] par) {
            Helpers.Spawn(par[0]);
            UrsaLog.Success(par[0] + " was spawned.");
        }
    }
    public static class TransformDeepChildExtension {
        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName) {
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent) {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }

    public class ConsoleVarAttribute : Attribute {
        public string Name;
        public string Description;

        public ConsoleVarAttribute(string name) {
            Name = name;
            Description = "nondescript";
        }

        public ConsoleVarAttribute(string name, string description) {
            Name = name;
            Description = description;
        }
    }
}