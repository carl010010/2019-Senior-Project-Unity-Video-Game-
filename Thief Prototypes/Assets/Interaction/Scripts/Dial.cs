using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dial : MonoBehaviour {

    public Transform m_dial;
    public int dialPos;

    public IEnumerator DialUp()
    {
        for (int i = 0; i < 18; i++)
        {
            m_dial.Rotate(0, 2, 0, Space.Self);
            yield return null;
        }
        dialPos = (dialPos + 1) % 10;
    }

    public IEnumerator DialDown()
    {
        for (int i = 0; i < 18; i++)
        {
            m_dial.Rotate(0, -2, 0, Space.Self);
            yield return null;
        }

        if (dialPos <= 0)
        {
            dialPos = 9;
        }
        else
        {
            dialPos--;
        }
    }
}
