namespace URSA.Configurator {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implement to create custom configurator systems
    /// </summary>
    public interface IConfiguratorSystem {
        /// <summary>
        /// Initial configuration on launch
        /// </summary>
        void ConfigureGlobal();
        /// <summary>
        /// Configuration repeated every time a scene switches
        /// </summary>
        void ConfigureLocal();
    }

}