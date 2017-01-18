#if UNITY_EDITOR
namespace pointcache.utility
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEditor;

    public class HierarchyVisualizer : MonoBehaviour
    {
        public bool hideMesh = true;

        public void ToggleMesh()
        {
            var mr = GetComponent<SkinnedMeshRenderer>();
            if (!mr)
            {
                mr = GetComponentInChildren<SkinnedMeshRenderer>();
            }
            mr.enabled = !mr.enabled;
        }

        public void On()
        {
            AddDrawers_Recursive(transform);
            var mr = GetComponent<SkinnedMeshRenderer>();
            if (!mr)
            {
                mr = GetComponentInChildren<SkinnedMeshRenderer>();
            }
            mr.enabled = !hideMesh;
            mr.gameObject.hideFlags = HideFlags.NotEditable;
        }
        public void Off()
        {
            RemoveDrawers_Recursive(transform);
            var mr = GetComponent<SkinnedMeshRenderer>();
            if (!mr)
            {
                mr = GetComponentInChildren<SkinnedMeshRenderer>();
            }
            mr.enabled = true;
            mr.gameObject.hideFlags = HideFlags.None;

        }
        void AddDrawers_Recursive(Transform tr)
        {
            if (tr.childCount > 0)
            {
                foreach (Transform t in tr)
                {
                    if (!t.gameObject.GetComponent<BoneDrawer>())
                        t.gameObject.AddComponent<BoneDrawer>();
                    AddDrawers_Recursive(t);
                }
            }
        }
        void RemoveDrawers_Recursive(Transform tr)
        {
            if (tr.childCount > 0)
            {
                foreach (Transform t in tr)
                {
                    var drawer = t.gameObject.GetComponent<BoneDrawer>();
                    if (drawer)
                        DestroyImmediate(drawer);
                    RemoveDrawers_Recursive(t);
                }
            }
        }
    }
}
#endif