namespace URSA.Utility {
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    public class ProjectInfo : ScriptableObject {

        public static ProjectInfo _current;
        public static ProjectInfo current
        {
            get {
                if (_current == null)
                    _current = Resources.Load("AdditionalData/ProjectInfo") as ProjectInfo;
                return _current;
            }
        }

        public float Version = 0.0f;
    }

}