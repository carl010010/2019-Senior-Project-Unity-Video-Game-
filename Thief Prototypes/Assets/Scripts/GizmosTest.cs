using System.Collections;
using System.Collections.Generic;
using GizmosEditors;
using UnityEngine;

public class GizmosTest : MonoBehaviour
{
    //Used to add the functions to the GizmosEditor during PlayMode
//    void Awake()
//    {
//        //GizmoComposite composite = new GizmoComposite(name, GizmoBoxes, GizmoLines);
//        //GizmosEditor.gizmoComposites.Add(composite);
//    }

//    private void OnDisable()
//    {
//        //Debug.Log("Diabaling");
//        //GizmosEditor.gizmoComposites.Clear();
//    }

//    private void OnEnable()
//    {
//        GizmosEditor.AddToOnDrawGizmos(name, GizmoBoxes, GizmoLines);
//        //GizmoComposite composite = new GizmoComposite(name, GizmoBoxes, GizmoLines);
//        //GizmosEditor.gizmoComposites.Add(composite);
//    }

//    //Used to add the functions to the GizmosEditor while in the editor
//#if UNITY_EDITOR
//    [UnityEditor.Callbacks.DidReloadScripts]
//    static void Test()
//    {
//        if (!Application.isPlaying)
//        {
//            GizmosTest[] a = FindObjectsOfType<GizmosTest>();

//            foreach (GizmosTest v in a)
//            {
//                GizmosEditor.AddToOnDrawGizmos(v.name, v.GizmoBoxes, v.GizmoLines);
//            }
//        }
//    }
//#endif
    [GizmoMethod]
    void GizmoBoxes()
    {
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(5, 5, 5));
        Gizmos.DrawCube(new Vector3(6, 0, 6), new Vector3(1, 11, 1));
        Gizmos.DrawCube(new Vector3(-6, 0, -16), new Vector3(0.5f, 0.5f, 10));
    }

    [GizmoMethod]
    void GizmoLines()
    {
        Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(5, 5, 5));
        Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(-5, 5, -5));
    }
}
