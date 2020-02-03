using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStim : StimResponse
{

	// Use this for initialization
	public void Start () {
        base.Initialize();
        AddStim(StimType.FIRE);
	}
}
