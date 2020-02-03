using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEditor;
using System;
using GizmosEditors;

public class AI_Movement : MonoBehaviour
{
    public bool StandingGuard = false;

    public GameObject ParentWaypoint;

    public List<Vector3> pathList = new List<Vector3>();
    public float distanceToActivePathPos;

    //public Transform goal;
    public bool isPaused;

    int pathPosNumber = 0;
    [HideInInspector]
    public NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent Component found");
        }

        if (ParentWaypoint == null)
        {
            Debug.LogError("No Path found found");
        }

        pathList.Clear();
        foreach (Transform t in ParentWaypoint.transform)
        {
            pathList.Add(t.transform.position);
        }

        if (StandingGuard)
        {
            agent.destination = ParentWaypoint.transform.position;
        }
        else if (pathList.Count > 1)
        {
            agent.destination = pathList[0];
        }
        else
        {
            Debug.LogError("pathList has less then 2 positions or must be Standing Guard");
        }
    }
    float time;
    public void Guard(bool wasVisable)
    {
        if (StandingGuard)
        {
            agent.destination = ParentWaypoint.transform.position;
            if (agent.velocity == Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, ParentWaypoint.transform.rotation, Time.deltaTime * 2f);
            }
            return;
        }


        if (agent.isOnOffMeshLink)
        {
            DoorManager door = agent.currentOffMeshLinkData.offMeshLink.gameObject.GetComponent<DoorManager>();
            if (door != null)
            {
                if (!door.IsOpen())
                {
                    door.OpenDoor();
                }
                else
                {
                    agent.autoTraverseOffMeshLink = true;
                    door.CloseDoor();
                    return;
                }
            }
        }
        else
        {
            agent.autoTraverseOffMeshLink = false;
        }

        if (pathList.Count > 1)
        {
            if (wasVisable == true)
            {
                if ((transform.position - pathList[pathPosNumber]).sqrMagnitude > (transform.position - pathList[IncrementWrapVaule(pathPosNumber, pathList.Count)]).sqrMagnitude)
                {
                    pathPosNumber = IncrementWrapVaule(pathPosNumber, pathList.Count);
                }
            }

            agent.destination = pathList[pathPosNumber];
            //If in range to the target path pos, set goal to the next path pos
            if ((transform.position - pathList[pathPosNumber]).sqrMagnitude < distanceToActivePathPos * distanceToActivePathPos)
            {
                pathPosNumber = IncrementWrapVaule(pathPosNumber, pathList.Count);
            }
        }
    }

    int IncrementWrapVaule(int value, int max)
    {
        return (value + 1) % max;
    }

    public void UpdateAgentDestination(Vector3 pos)
    {
        agent.destination = pos;
    }


#if UNITY_EDITOR

    [GizmoMethod]
    private void DrawPath()
    {
        Gizmos.DrawLine(ParentWaypoint.transform.forward + ParentWaypoint.transform.position, ParentWaypoint.transform.position);

        if (ParentWaypoint != null)
        {
            pathList.Clear();
            foreach (Transform t in ParentWaypoint.transform)
            {
                pathList.Add(t.transform.position);
            }

            for (int i = 0; i < pathList.Count; i++)
            {
                if (i == pathPosNumber)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawWireSphere(pathList[i], .2f);
            }

            for (int i = 0; i < pathList.Count; i++)
            {
                if (i == pathPosNumber)
                {
                    Gizmos.color = Color.green;
                    if (pathPosNumber == 0)
                    {
                        Gizmos.DrawLine(pathList[0], pathList[pathList.Count - 1]);
                    }
                    else
                    {
                        Gizmos.DrawLine(pathList[pathPosNumber], pathList[pathPosNumber - 1]);
                    }
                }
                else if (i == 0)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(pathList[0], pathList[pathList.Count - 1]);
                }
                else
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(pathList[i], pathList[i - 1]);
                }
            }
        }
    }

    [GizmoMethod]
    private void DrawNavMeshPath()
    {
        if (agent == null)
            return;

        if (!agent.pathPending)
        {
            Gizmos.color = Color.white;

            Vector3[] path = agent.path.corners;

            for (int i = 0; i < path.Length - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
#endif
}