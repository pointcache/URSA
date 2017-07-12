namespace URSA.Utility {
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    public class ProjectInfo : ScriptableObject {

        public static ProjectInfo _current;
        public static ProjectInfo Current
        {
            get {
                if (_current == null)
                    _current = Resources.Load(URSA.Internal.URSAConstants.PATH_ADDITIONAL_DATA +  "/ProjectInfo") as ProjectInfo;
                return _current;
            }
        }

        public float Version = 0.0f;
    }

}