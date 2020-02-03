using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour {

    public bool toggle = false;
    public UnityEvent m_SwitchEventON;
    public UnityEvent m_SwitchEventOFF;



    public void Toggle()
    {
        if (toggle)
            m_SwitchEventON.Invoke();
        else
            m_SwitchEventOFF.Invoke();

        toggle = !toggle;
    }
}
