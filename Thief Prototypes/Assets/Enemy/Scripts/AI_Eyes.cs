using System.Collections;
using System.Collections.Generic;
using GizmosEditors;
using UnityEngine;

public class AI_Eyes : MonoBehaviour
{
    public float viewDistance;
    [Range(0, 360)]
    public float viewAngle;

    private bool debugline;

    public bool CanSeeGoal(Vector3 goalPos)
    {
        if (Vector3.Distance(transform.position, goalPos) < viewDistance)
        {
            Vector3 dirToPlayer = (goalPos - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                RaycastHit hit;
                if (!Physics.Linecast(transform.position, goalPos, out hit))
                {
                    debugline = true;
                    return true;
                }
            }
        }
        debugline = false;
        return false;
    }
#if UNITY_EDITOR
    [GizmoMethod]
    private void DrawViewCone()
    {
        Vector3 position = transform.position;

        if (debugline == true)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, AI_Brain.BlackBoard.b_player.transform.position);
        }

        Gizmos.color = Color.white;

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        UnityEditor.Handles.DrawWireArc(position, Vector3.up, viewAngleA, viewAngle, viewDistance);
        Gizmos.DrawLine(position, position + (viewAngleA * viewDistance));
        Gizmos.DrawLine(position, position + (viewAngleB * viewDistance));
    }

    [GizmoMethod]
    private void DrawCanSeeRay()
    {
        Vector3 position = transform.position;

        if (debugline == true)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(position, AI_Brain.BlackBoard.b_player.transform.position);
    }
#endif

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
