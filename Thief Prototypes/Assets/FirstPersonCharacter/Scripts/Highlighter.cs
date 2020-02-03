using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour
{



    private HighlightOnHover Highlighted;
    // Use this for initialization
    public void highlight(RaycastHit? hit)
    {
        if (hit != null)
        {
            GameObject hitObject = hit.Value.collider.gameObject;

            if (hitObject.layer == LayerConstants.Dial || hitObject.layer == LayerConstants.Switch
            || hitObject.layer == LayerConstants.Door)
            {
                HighlightOnHover temp = hitObject.GetComponent<HighlightOnHover>();

                if(temp == null)
                {
                    Debug.Log(hitObject.name, hitObject);
                }
                else if (Highlighted == null)
                {
                    Highlighted = temp;
                    Highlighted.ToggleHighLight();
                }
                else if (Highlighted != temp)
                {
                    //Turn old item highlight off
                    Highlighted.ToggleHighLight();
                    Highlighted = temp;
                    //Turn new item highlight on
                    Highlighted.ToggleHighLight();
                }

            }
            else if (Highlighted != null)
            {
                Highlighted.ToggleHighLight();
                Highlighted = null;
            }
        }
        else if (Highlighted != null)
        {
            Highlighted.ToggleHighLight();
            Highlighted = null;
        }
    }
}
