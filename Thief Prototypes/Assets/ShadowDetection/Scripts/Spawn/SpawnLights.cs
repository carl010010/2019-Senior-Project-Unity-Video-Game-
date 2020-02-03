using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLights : MonoBehaviour
{

    public DistanceToLights m_DistanceScript;


    public int Lights = 0;
    public int height = 5;
    //public MonoBehaviour test;
    public GameObject lightToSpawn = null;

    Bounds bounds;
    // Use this for initialization
    void Start()
    {
        bounds = GetComponent<BoxCollider>().bounds;


        for (int i = 0; i < Lights; i++)
        {
            GameObject Object = Instantiate(lightToSpawn, GetRandomPositionToSpawn(bounds.min, bounds.max, height), Quaternion.identity);
            Object.transform.parent = this.transform;
            Object.name = lightToSpawn.name + i.ToString();
        }

        if (m_DistanceScript == null)
        {
            Debug.LogError("Did you forget to plug in m_DistanceScript?", this);
        }
        else
        {
			m_DistanceScript.Reset();
        }
    }


    Vector3 GetRandomPositionToSpawn(Vector3 start, Vector3 end, int _height)
    {
        return new Vector3(Random.Range(start.x, end.x),
                           Random.Range(end.y, _height),
                           Random.Range(start.z, end.z));
    }
}
