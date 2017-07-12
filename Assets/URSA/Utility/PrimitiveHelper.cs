namespace URSA.Utility {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public static class PrimitiveHelper {
        private static Dictionary<PrimitiveType, Mesh> primitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

        public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider) {
            if (withCollider) { return GameObject.CreatePrimitive(type); }

            GameObject gameObject = new GameObject(type.ToString());
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = PrimitiveHelper.GetPrimitiveMesh(type);
            gameObject.AddComponent<MeshRenderer>();

            return gameObject;
        }

        public static Mesh GetPrimitiveMesh(PrimitiveType type) {
            if (!PrimitiveHelper.primitiveMeshes.ContainsKey(type)) {
                PrimitiveHelper.CreatePrimitiveMesh(type);
            }

            return PrimitiveHelper.primitiveMeshes[type];
        }

        private static Mesh CreatePrimitiveMesh(PrimitiveType type) {
            GameObject gameObject = GameObject.CreatePrimitive(type);
            Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

#if UNITY_EDITOR
            GameObject.DestroyImmediate(gameObject);

#else
        GameObject.Destroy(gameObject);
#endif


            PrimitiveHelper.primitiveMeshes[type] = mesh;
            return mesh;
        }
    }

}