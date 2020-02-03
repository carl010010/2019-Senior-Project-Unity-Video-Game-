using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCubes : MonoBehaviour {

	public int CubesToSpawn;
    public int maxHeight;
    public int minHeight;
    public GameObject cube;

    Bounds bounds;


    void Start()
    {
        bounds = GetComponent<BoxCollider>().bounds;

        for(int i = 0; i < CubesToSpawn; i++)
        {
            Vector3 spawnPos = GetRandomPositionToSpawn(bounds.min, bounds.max, minHeight, maxHeight);
            float y = spawnPos.y;

            spawnPos = new Vector3(spawnPos.x, y / 2, spawnPos.z);

            var cubeI = Instantiate(cube, spawnPos, Quaternion.identity);
            cubeI.transform.localScale = new Vector3(cube.transform.localScale.x * Random.Range(1, 4), y, cube.transform.localScale.z * Random.Range(1, 4));
            cubeI.transform.parent = this.transform;
        }



    }


    Vector3 GetRandomPositionToSpawn(Vector3 start, Vector3 end, int minHeight, int maxHeight)
    {
        return new Vector3(Random.Range(start.x, end.x),
                           Random.Range(end.y + minHeight, maxHeight),
                           Random.Range(start.z, end.z));
    }
}
