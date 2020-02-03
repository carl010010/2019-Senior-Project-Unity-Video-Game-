using UnityEngine;

public class test : MonoBehaviour
{
    public AnimationCurve MoveCurve;

    public Vector3 _target;
    public Vector3 _startPoint;
    public float _animationTimePosition;

    public bool run;

    private void Start()
    {
        UpdatePoint();
        run = true;
    }

    private void Update()
    {

        if (run && _animationTimePosition < 1)
        {
            _animationTimePosition += Time.deltaTime;
            transform.position = Spring(_startPoint, _target, _animationTimePosition);
        }
        else if(_animationTimePosition > 0)
        {
            _animationTimePosition = 0;
            UpdatePoint();
            run = false;
        }
        //run = false;

    }

    private void UpdatePoint()
    {
        _startPoint = transform.position;
        _target = _startPoint + Random.insideUnitSphere *3;
    }

    public static float Spring(float from, float to, float time)
    {
        time = Mathf.Clamp01(time);
        time = (Mathf.Sin(time * Mathf.PI * (.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + (1.2f * (1f - time)));
        return from + (to - from) * time;
    }

    public static Vector3 Spring(Vector3 from, Vector3 to, float time)
    {
        return new Vector3(Spring(from.x, to.x, time), Spring(from.y, to.y, time), Spring(from.z, to.z, time));
    }
}
