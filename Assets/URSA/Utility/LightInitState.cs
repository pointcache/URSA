namespace URSA.Utility {

    using UnityEngine;

    /// <summary>
    /// Used to store data about intial state of the light in scene before you launch it.
    /// Used to return back to the state the light was before, after changing settings.
    /// </summary>
    public class LightInitState : MonoBehaviour {

        public LightShadows init;
    }

}