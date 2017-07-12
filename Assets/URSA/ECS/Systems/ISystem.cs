using UnityEngine;
using System;
using System.Collections.Generic;

public interface ISystem  {
    MonoBehaviour Owner { get; }
    void OrderedOnEnable();
    void OrderedFixedUpdate();
    void OrderedUpdate();
}
