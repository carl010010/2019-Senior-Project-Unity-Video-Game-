using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMovementTest : MonoBehaviour
{
    public float m_MaxDistanceItemCheck = 5;

    // Use this for initialization
    void Start () {
		
	}

    readonly private Vector3 up = new Vector3(0,1,0);
	// Update is called once per frame
	void Update () {


        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit) && (transform.position - hit.point).sqrMagnitude < m_MaxDistanceItemCheck * m_MaxDistanceItemCheck)
        {
            Vector3 pos = hit.point;
            //hit.normal
            if (Input.GetMouseButton(1))
            {
                //Debug.DrawLine(pos, transform.position + transform.forward * m_MaxDistanceItemCheck);
                Debug.DrawRay(pos - hit.normal  + up,up * -1);
                if (Physics.CheckBox(pos - hit.normal + up, Vector3.one))
                {

                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * m_MaxDistanceItemCheck);
    }
}
