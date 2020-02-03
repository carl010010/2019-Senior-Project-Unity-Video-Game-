using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Safe : DoorManager
{
    
    public Dial[] dials;
    public int[] correctDialPos;

    public new void Start()
    {
        base.Start();
        if (correctDialPos.Length != dials.Length)
        {
            Debug.LogError("correctDialPos.Length != dials.Length for" + this.name);
        }
    }

    public override void OpenDoor(int KeyValue)
    {
        if (correctDialPos.Length != dials.Length)
        {
            Debug.LogError("correctDialPos.Length != dials.Length for" + this.name);
        }

        for (int i = 0; i < dials.Length; i++)
        {
            if(dials[i].dialPos != correctDialPos[i])
            {
                //TODO add sound/wiggle the handle
                return;//Return if the dials arent in the correct places
            }
        }
        base.AnimateDoors("Open", true);
    }
}
