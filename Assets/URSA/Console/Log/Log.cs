
namespace URSA {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using SmartConsole;

    public static class Log {
        const bool output_unity_log = true;
#if !UNITY_EDITOR
#endif
        public static bool realtime = false;
        public static List<string> currentLog = new List<string>();

        const string idcol = "<color=#1676d0>>        ";
        const string objnamecol = "<color=white>: ";
        public static string hash(this object obj) {
            return idcol + obj.GetHashCode() + "</color>";
        }
        public static void log(this Entity ent) {
            if (!ent)
                return;
            Print("INFO: " + ent.name);
            Print("database_ID: " + ent.database_ID);
            Print("instance_ID: " + ent.instance_ID);
            Print("blueprint_ID: " + ent.blueprint_ID);
        }

        public static void DumpLog() {
            SerializationHelper.Serialize(currentLog, Application.dataPath + "/ursa_log.txt", true);
            if (!realtime)
                Console.WriteLine("dumped " + currentLog.Count + " lines of log");
        }

        public static string record(string msg) {

            currentLog.Add(msg);
            if (realtime) {
                DumpLog();
            }
            return msg;
        }

        public static void Print(object message) {
            string msg = message.ToString();

#if UNITY_EDITOR
            if (output_unity_log) Debug.Log(msg);
#else
            record(msg);
            SmartConsole.Log(msg, SmartConsole.myLogType.log);
#endif

        }
        public static void Print(object message, UnityEngine.Object obj) {
            string msg = message.ToString();
#if UNITY_EDITOR
            if (output_unity_log) Debug.Log(msg, obj);
#else
            record(msg);
            SmartConsole.Log(msg, SmartConsole.myLogType.log);
#endif
        }
        public static void Error(object message) {
            string msg = message.ToString();
#if UNITY_EDITOR
            if (output_unity_log) Debug.LogError(msg);
#else
            
            record(msg);
            SmartConsole.Log(msg, SmartConsole.myLogType.error);
#endif
        }
        public static void Error(object message, UnityEngine.Object obj) {
            string msg = message.ToString();
#if UNITY_EDITOR
            if (output_unity_log) Debug.LogError(msg, obj);
#else
            record(msg);
            SmartConsole.Log(msg, SmartConsole.myLogType.error);
#endif
        }
        public static void Warning(object message) {
            string msg = message.ToString();
#if UNITY_EDITOR
            if (output_unity_log) Debug.LogWarning(msg);
#else
            record(msg);
            SmartConsole.Log(msg, SmartConsole.myLogType.warning);
#endif
        }
        public static void Warning(object message, UnityEngine.Object obj) {
            string msg = message.ToString();
#if UNITY_EDITOR
            if (output_unity_log) Debug.LogWarning(msg, obj);
#else
            record(msg);
            SmartConsole.Log(msg, SmartConsole.myLogType.warning);
#endif
        }
        public static void Success(object message) {
            string msg = message.ToString();
#if UNITY_EDITOR
            if (output_unity_log) Debug.Log(msg);
#else
            record(msg);
            SmartConsole.Log(msg, SmartConsole.myLogType.confirmation);
#endif
        }
        public static void Success(object message, UnityEngine.Object obj) {
            string msg = message.ToString();
#if UNITY_EDITOR
            if (output_unity_log) Debug.Log(msg, obj);
#else
            record(msg);
            SmartConsole.Log(msg, SmartConsole.myLogType.confirmation);
#endif
        }
    }
}