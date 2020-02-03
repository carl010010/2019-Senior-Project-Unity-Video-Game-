using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SeachingBehavior : MonoBehaviour
{
    public Vector3 pos;
    public Vector3 velocity;


    public float searchRadius = 5;
    public float searchPathLength = 5;
    //public Vector3 SearchPos;
    [SerializeField]
    private Vector3[] postions = new Vector3[8];

    NavMeshPath[] paths = new NavMeshPath[8];

    [ContextMenu("FindSearchPositions")]
    void FindSearchPositions()
    {
        Vector3 SearchPos = pos + velocity * .5f;

        Vector3 Temp;

        for(int i = 0; i < postions.Length; i++)
        {
            paths[i] = new NavMeshPath();
        }

        for (int i = 0; i < 8; i++)
        {
            float distance = 0;
            do
            {
                distance = 0;
                Temp = new Vector3(Random.Range(SearchPos.x - searchRadius, SearchPos.x +searchRadius), SearchPos.y, Random.Range(SearchPos.z - searchRadius, SearchPos.z + searchRadius));
                NavMesh.CalculatePath(SearchPos, Temp, NavMesh.AllAreas, paths[i]);
                distance = GetPathLength(paths[i]);

            } while (distance > searchPathLength || paths[i].status == NavMeshPathStatus.PathInvalid);
            postions[i] = Temp;
        }
    }


    public static float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }

    private void OnDrawGizmos()
    {
        foreach (NavMeshPath nav in paths)
        {
            if (nav != null)
            {
                Gizmos.color = Color.white;

                Vector3[] path = nav.corners;

                for (int i = 0; i < path.Length - 1; i++)
                {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }

        for (int i = 0; i < 8; i++)
        {
            Gizmos.DrawWireSphere(postions[i], 0.5f);
        }


        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 1);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos + velocity * .5f, 1);
    }
}
