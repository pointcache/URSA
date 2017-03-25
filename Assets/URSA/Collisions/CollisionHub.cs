using UnityEngine;
using System;
using System.Collections.Generic;

//https://github.com/pointcache/URSA/wiki/Collision-Hub

public static class CollisionHub<T> where T : ComponentBase {

    public static event Action<T, Collision> OnCollisionEnter = delegate { };
    public static event Action<T, Collision> OnCollisionStay = delegate { };
    public static event Action<T, Collision> OnCollisionExit = delegate { };
    public static event Action<T, Collider> OnTriggerEnter = delegate { };
    public static event Action<T, Collider> OnTriggerStay = delegate { };
    public static event Action<T, Collider> OnTriggerExit = delegate { };


    public static void CollisionEnter(T sender) {
        CollisionEnter(sender, null);
    }

    public static void CollisionEnter(T sender, Collision collision) {
        OnCollisionEnter(sender, collision);
    }

    public static void CollisionStay(T sender) {
        CollisionStay(sender, null);
    }

    public static void CollisionStay(T sender, Collision collision) {
        OnCollisionStay(sender, collision);
    }

    public static void CollisionExit(T sender) {
        CollisionExit(sender, null);
    }

    public static void CollisionExit(T sender, Collision collision) {
        OnCollisionExit(sender, collision);
    }

    public static void TriggerEnter(T sender, Collider collider) {
        OnTriggerEnter(sender, collider);
    }

    public static void TriggerStay(T sender, Collider collider) {
        OnTriggerEnter(sender, collider);
    }

    public static void TriggerExit(T sender, Collider collider) {
        OnTriggerEnter(sender, collider);
    }
}
