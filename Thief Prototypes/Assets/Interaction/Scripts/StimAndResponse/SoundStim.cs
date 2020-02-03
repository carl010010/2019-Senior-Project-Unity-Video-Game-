using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStim : MonoBehaviour
{
    [SerializeField]
    private float _radius;

    public float radius;
    public float lifeTime;
    private float timer;

    public void Start()
    {
        if (lifeTime > 0)
        {
            AI_Brain.BlackBoard.b_sounds.Add(this);
        }
        else
        {
            Debug.Log("Sound Object was created with a lifttime of zero " + name);
            Destroy(gameObject);
        }

    }

    public void Update()
    {
        if (timer < lifeTime)
        {
            timer += Time.deltaTime;
            _radius = radius * (1- timer/lifeTime);
        }
        else
            Destroy(gameObject);
    }

    public float GetRadius()
    {
        return _radius;
    }

    private void OnDisable()
    {
        AI_Brain.BlackBoard.b_sounds.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,_radius);
    }


}
