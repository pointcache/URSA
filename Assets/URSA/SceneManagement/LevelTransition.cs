namespace URSA.SceneManagement {
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Object holding data that will be passed when requesting scene change, allows to specify additional 
    /// info, for example which entry point to use.
    /// </summary>
    [Serializable]
    public class SceneTransition {
        public SceneData scenedata;
        public string EntryPoint;
    }

}