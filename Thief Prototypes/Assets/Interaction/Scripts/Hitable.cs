using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitable : MonoBehaviour
{

    [System.Serializable]
    public class Vector3Event : UnityEvent<Vector3>
    {
    }
    public Vector3Event m_hit;

    private void OnCollisionEnter(Collision collision)
    {
        m_hit.Invoke(collision.contacts[0].point);

        this.enabled = false;
    }
}
