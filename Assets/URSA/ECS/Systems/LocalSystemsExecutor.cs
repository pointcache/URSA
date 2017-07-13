namespace URSA.ECS.Systems {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using URSA.Utility;

    /// <summary>
    /// Enforces ordered execution of unity updates and onEnable through SystemBase
    /// Only works with direct children, intended for usage with URSA systems
    /// </summary>
    public class LocalSystemsExecutor : SystemsExecutor {

    }
}