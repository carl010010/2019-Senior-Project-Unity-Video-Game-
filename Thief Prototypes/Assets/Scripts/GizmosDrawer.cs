using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using GizmosEditors;
#if UNITY_EDITOR
[InitializeOnLoad]
public class GizmosDrawer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        foreach(Action a in GizmosEditor.actions)
        {
            if(a.Target.Equals(null))
            {
                Debug.LogWarning("Trying to call a function with a broken link to the origanl object");
            }

            if(!a.Target.Equals(null) && a != null && a.Method != null)
            {
                a.Invoke();
            }
        }
    }
}
#endif