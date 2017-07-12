namespace URSA.Config.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using URSA.Internal;

    public static class ConfigSystemTools {
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_CONFIG + "/LoadSelectedConfig(Editor)")]
        public static void LoadSelectedConfig() {
            var go = Selection.activeGameObject;
            if (!go)
                return;

            var cfg = go.GetComponent<ConfigBase>();
            if (!cfg)
                return;

            ConfigSystem.TryLoadSingle(cfg);
        }

        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_CONFIG + "/SaveSelectedConfig(Editor)")]
        public static void SaveSelectedConfig() {
            var go = Selection.activeGameObject;
            if (!go)
                return;

            var cfg = go.GetComponent<ConfigBase>();
            if (!cfg)
                return;

            ConfigSystem.TrySaveSingle(cfg);
        }
    }

}