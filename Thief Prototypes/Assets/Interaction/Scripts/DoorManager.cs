using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Animator))]
public class DoorManager : MonoBehaviour
{
    public bool IsLocked = false;
    public int keyValue;
    [Space]
    public int[] lockpickValues;
    public float timeToLockpick;

    Animator animator;
    [Space]
    public bool isDoorOpen; //Changed by animation
    public DoorSounds doorSounds;
    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    float oldTimer;
    float lockPicktimer;
    float updateTimer;
    int tumblerPos;

    private void Update()
    {
        if (tumblerPos != 0 || (System.Math.Abs(oldTimer) > 0 && System.Math.Abs(oldTimer - lockPicktimer) < 0.01))
        {
            updateTimer += Time.deltaTime;
            if (updateTimer > 1.3f)
            {
                lockPicktimer = 0;
                //TODO Make failure Sound
                AudioSourcePool.PlayAtPoint(doorSounds.KeyRemove, transform.position);
                tumblerPos = 0;
            }
        }
        else
        {
            updateTimer = 0;
        }

        oldTimer = lockPicktimer;
    }


    float soundTimer;
    int oldLockPick;
    public void PickDoor(int lockpickValue)
    {
        updateTimer = 0;
        if (lockpickValues.Length > 0)
        {
            if (tumblerPos == lockpickValues.Length)
            {
                //TODO Make unlock sound
                AudioSourcePool.PlayAtPoint(doorSounds.DoorUnlock, transform.position);
                OpenDoor();
                tumblerPos = 0;
                oldTimer = lockPicktimer = 0;
            }
            else if (lockpickValue == lockpickValues[tumblerPos])
            {
                lockPicktimer += Time.deltaTime;
                if (lockPicktimer > timeToLockpick)
                {
                    //TODO Make success sound
                    AudioSourcePool.PlayAtPoint(doorSounds.KeyInsert, transform.position);
                    tumblerPos++;
                    lockPicktimer = 0;

                }
                else
                {
                    //TODO Make lockpicking sound
                    AudioSourcePool.PlayAtPoint(doorSounds.LockPicking, transform.position);
                }
            }
            else
            {
                lockPicktimer = 0;
                //TODO Make failure Sound
            }
        }
        else if (!IsLocked)
        {
            OpenDoor();
        }
        else
        {
            //TODO Make failure Sound
            AudioSourcePool.PlayAtPoint(doorSounds.KeyRemove, transform.position);
        }
    }


    public virtual void OpenDoor(int KeyValue)
    {
        if (!IsLocked || (IsLocked && KeyValue == keyValue))
        {
            //TODO Make success sound
            AudioSourcePool.PlayAtPoint(doorSounds.KeyInsert, transform.position);
            AudioSourcePool.PlayAtPoint(doorSounds.DoorUnlock, transform.position);
            OpenDoor();
        }
        else
        {
            //TODO Make failure Sound
            AudioSourcePool.PlayAtPoint(doorSounds.DoorLocked, transform.position);
        }

    }

    public void OpenDoor()
    {
        //TODO Make Open sound
        AudioSourcePool.PlayAtPoint(doorSounds.DoorClose, transform.position);
        AnimateDoors("Open", true);
    }

    public void CloseDoor()
    {
        //TODO Make Close sound
        AudioSourcePool.PlayAtPoint(doorSounds.DoorClose, transform.position);
        AnimateDoors("Open", false);
    }

    public bool IsOpen()
    {
        return isDoorOpen;
    }

    protected void AnimateDoors(string direction, bool open)
    {
        animator.SetBool(direction, open);
    }



    //public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    //{
    //    Vector3 dir = point - pivot; // get point direction relative to pivot
    //    dir = Quaternion.Euler(angles) * dir; // rotate it
    //    point = dir + pivot; // calculate rotated point
    //    return point; // return it
    //}

    //public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angles)
    //{
    //    Vector3 dir = point - pivot; // get point direction relative to pivot
    //    dir = angles * dir; // rotate it
    //    point = dir + pivot; // calculate rotated point
    //    return point; // return it
    //}
}
