namespace URSA.Utility {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public static class TransformUtilities {

        public static Vector3 GetAxis(this Axis axis) {
            switch (axis) {
                case Axis.x:
                    return Vector3.right;

                case Axis.y:
                    return Vector3.up;

                case Axis.z:
                    return Vector3.forward;
            }
            return Vector3.zero;
        }

        public static List<Transform> GetTransformsInOrder(this GameObject go) {
            var tr = go.transform;
            List<Transform> list = new List<Transform>(tr.childCount);
            foreach (Transform t in tr) {
                list.Add(t);
            }

            return list;
        }

        public static void GetChildren(this Transform tr, List<Transform> container) {
            if (container == null) {
                Debug.LogError("List is null");
                return;
            }
            foreach (Transform child in tr) {
                container.Add(child);
            }
        }

        public static void DestroyChildren(this Transform tr) {
            if (tr.childCount == 0)
                return;
            List<Transform> list = new List<Transform>();
            foreach (Transform child in tr) {
                list.Add(child);
            }
            int count = list.Count;
            for (int i = 0; i < count; i++) {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(list[i].gameObject);
#else
            GameObject.Destroy(list[i].gameObject);

#endif
            }
        }

        public static void GetAllChildren(this Transform tr, List<Transform> container) {
            if (container == null) {
                Debug.LogError("Attempt to use null List");
                return;
            }
            _GetAllChildrenRecursive(tr, container);
        }

        private static void _GetAllChildrenRecursive(Transform tr, List<Transform> container) {
            foreach (Transform child in tr) {
                container.Add(child);
                _GetAllChildrenRecursive(child, container);
            }
        }

        public static void SetAllChildren(this Transform tr, bool state) {
            foreach (Transform t in tr) {
                t.gameObject.SetActive(state);
            }
        }

        public static Vector2 GetRealMax(this RectTransform rect) {
            float x = rect.position.x + (rect.sizeDelta.x * (1 - rect.pivot.x));
            float y = rect.position.y + (rect.sizeDelta.y * (1 - rect.pivot.y));
            return new Vector2(x, y);
        }

        public static Vector2 GetRealMin(this RectTransform rect) {
            float x = (rect.position.x - (rect.sizeDelta.x * rect.pivot.x));
            float y = rect.position.y - (rect.sizeDelta.y * rect.pivot.y);
            return new Vector2(x, y);
        }

        public static void DisableAllChildren(this Transform tr) {
            foreach (Transform t in tr) {
                t.gameObject.SetActive(false);
            }
        }

        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName) {
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent) {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static void ZeroOut(this Transform tr) {
            tr.position = Vector3.zero;
            tr.rotation = Quaternion.identity;
        }

        public static void ZeroOutLocal(this Transform tr) {
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;
        }

        public static Vector2[] GetScreenCorners(this RectTransform tr) {
            Vector3[] corners = new Vector3[4];
            tr.GetWorldCorners(corners);
            Vector2[] s_corners = new Vector2[4];
            for (int i = 0; i < 4; i++) {
                s_corners[i].x = corners[i].x;
                s_corners[i].y = corners[i].y;
            }

            return s_corners;
        }
    }

}