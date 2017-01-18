#if UNITY_EDITOR
namespace pointcache.utility
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    public class BoneDrawer : MonoBehaviour
    {
        const float rad = 0.05f;
        private void OnDrawGizmos()
        {
            if (transform.parent != null)
            {
                Gizmos.DrawLine(transform.position, transform.parent.position);
                
                Gizmos.DrawSphere(transform.position, rad);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Selection.activeGameObject == gameObject && transform.childCount > 0)
            {
                Handles.Label(transform.position, "-----------  " + name);
                Gizmos.color = Color.yellow;

                foreach (Transform t in transform)
                {
                    Gizmos.DrawLine(transform.position, t.position);
                    Gizmos.DrawWireSphere(transform.position, rad + 0.01f);
                    Gizmos.DrawWireSphere(t.position, rad + 0.01f);
                }


            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, rad - 0.01f);
                if(Selection.activeGameObject != transform.parent)
                    Gizmos.DrawLine(transform.position, transform.parent.position);
            }

        }
    }
}
#endif