using System.Collections;
using System.Collections.Generic;
using GizmosEditors;
using UnityEngine;


public class DistanceToLights : MonoBehaviour
{
    [Range(0, 1)]
    public float radiusPerent;

    public enum LightVisibility { OutOfRange, Visible, InVisible };

    [System.Serializable]
    public struct t_Lights
    {
        public Light light;
        public LightVisibility lightVisibility;
        public float RangeSqr;
    }

    public t_Lights[] m_lights;

    public float LightPercentage;


    public void Reset()
    {
        Light[] lightTransforms = FindObjectsOfType(typeof(Light)) as Light[];
        int length = lightTransforms.Length;
        int count = 0;
        for (int i = 0; i < length; i++)
        {
            if (lightTransforms[i].type == LightType.Point)
            {
                count++;
            }
        }

        m_lights = new t_Lights[count];

        for (int a = 0, b = 0; a < length; a++)
        {
            if (lightTransforms[a].type == LightType.Point)
            {
                m_lights[b].light = lightTransforms[a];
                m_lights[b].RangeSqr = lightTransforms[a].range * lightTransforms[a].range;
                b++;
            }
        }
    }
    void Awake()
    {
        Reset();
    }

    private void Update()
    {
        LightPercentage = 0;

        Vector3 position = transform.position;

        for (int i = 0; i < m_lights.Length; i++)
        {
            t_Lights t_Light = m_lights[i];

            if (!t_Light.light.enabled)
            {
                t_Light.lightVisibility = LightVisibility.OutOfRange;
            }
            else
            {
                Vector3 lightPos = t_Light.light.transform.position;
                float DistanceToLightsqrMagnitude = (position - lightPos).sqrMagnitude;
                if (DistanceToLightsqrMagnitude < t_Light.RangeSqr)
                {
                    if (Physics.Linecast(lightPos, position))
                    {
                        t_Light.lightVisibility = LightVisibility.InVisible;
                    }
                    else
                    {
                        if(DistanceToLightsqrMagnitude < t_Light.RangeSqr * radiusPerent)
                        {
                            LightPercentage = 100;
                        }
                        else 
                        {
                            float normal = Mathf.InverseLerp(0, 1- 1 * radiusPerent, 1.0f - (DistanceToLightsqrMagnitude / t_Light.RangeSqr));
                            float temp = 100 * Mathf.Lerp(0, 1, normal);

                            //float temp = 100.0f * Mathf.Lerp(0, 1, 1.0f - (DistanceToLightsqrMagnitude / t_Light.RangeSqr));
                            if (temp > LightPercentage)
                                LightPercentage = temp;
                        }
                        t_Light.lightVisibility = LightVisibility.Visible;
                    }
                }
                else
                {
                    t_Light.lightVisibility = LightVisibility.OutOfRange;
                }
            }

            m_lights[i] = t_Light;
        }
     }

    [GizmoMethod]
    private void DrawLightVisability()
    {
        Gizmos.color = Color.green;

        if (m_lights != null)
        {
            foreach (t_Lights t in m_lights)
            {
                if (t.lightVisibility != LightVisibility.OutOfRange)
                {
                    if (t.lightVisibility == LightVisibility.Visible)
                        Gizmos.color = Color.green;
                    else if (t.lightVisibility == LightVisibility.InVisible)
                        Gizmos.color = Color.red;

                    Gizmos.DrawLine(transform.position, t.light.transform.position);
                }
            }
        }
    }

    [GizmoMethod]
    private void DrawLightRange()
    {
        Gizmos.color = Color.green;

        if (m_lights != null)
        {
            foreach (t_Lights t in m_lights)
            {
                if (t.lightVisibility != LightVisibility.OutOfRange)
                {
                    if (t.lightVisibility == LightVisibility.Visible)
                    {
                        Utils.DebugUtilities.DrawCircle(t.light.transform.position, t.light.range);
                        Utils.DebugUtilities.DrawCircle(t.light.transform.position, t.light.range * radiusPerent);
                    }
                }
            }
        }
    }
}
