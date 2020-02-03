using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GizmosEditors;

public class PathEditor : MonoBehaviour
{

    [GizmoMethod(1)]
    private void PathPoints()
    {
        if (transform.childCount == 0)
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        else
        {
            Gizmos.color = Color.white;
            foreach (Transform t in transform)
            {
                Gizmos.DrawWireSphere(t.position, 0.5f);
            }
        }
    }
}
