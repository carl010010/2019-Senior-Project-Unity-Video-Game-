using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimResponse : MonoBehaviour
{

    public enum StimType { SOUND, FIRE };

    public List<StimType> Stims;
    Dictionary<StimType, Action> GlobalStims = new Dictionary<StimType, Action>();


    // Use this for initialization
    public void Initialize()
    {
        GlobalStims.Add(StimType.SOUND, SoundResponse);
        GlobalStims.Add(StimType.FIRE, FireResponse);
    }

    private void OnTriggerEnter(Collider _collider)
    {
        Debug.Log("Trigger");

        var stims = _collider.GetComponent<StimResponse>().Stims;

        if (stims != null)
        {
            foreach (StimType s in stims)
            {
                if (GlobalStims.ContainsKey(s))
                {
                    GlobalStims[s].Invoke();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StimResponse stim = collision.gameObject.GetComponent<StimResponse>();
        if (stim != null)
        {
            List<StimType> stims = stim.Stims;
            foreach (StimType s in stims)
            {
                if (GlobalStims.ContainsKey(s))
                {
                    GlobalStims[s].Invoke();
                }
            }
        }
    }

    virtual protected void SoundResponse()
    {}

    virtual protected void FireResponse()
    {}

    protected void AddStim(StimType arg)
    {
        if (GlobalStims.ContainsKey(arg) && !Stims.Contains(arg))
        {
            Stims.Add(arg);
        }
    }
}
