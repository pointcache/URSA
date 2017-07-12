namespace URSA {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class SystemBase : MonoBehaviour {
        internal virtual void OrderedOnEnable() { }
        internal virtual void OrderedFixedUpdate() { }
        internal virtual void OrderedUpdate() { }
    }

}