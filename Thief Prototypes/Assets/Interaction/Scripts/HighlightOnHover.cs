using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class HighlightOnHover : MonoBehaviour
{
    [Range(0, 1)]
    public float brightness = 0.4f;
    public Material highlightMaterial;

    public GameObject[] gameObjects;

    private List<MeshRenderer> MeshRenderers = new List<MeshRenderer>();
    private List<Color> colors = new List<Color>();
    private List<Material> materials  = new List<Material>();

    void Start()
    {
        if(highlightMaterial == null)
        {
            Debug.LogError("No highlightMaterial found on " + name, this);
        }

        if (gameObjects.Length != 0)
        {
            foreach (GameObject t in gameObjects)
            {
                MeshRenderer temp = t.GetComponent<MeshRenderer>();

                if (temp != null)
                {
                    MeshRenderers.Add(temp);
                    colors.Add(temp.material.color);
                    materials.Add(temp.material);
                }
            }
        }
        else if (MeshRenderers.Count == 0)
        {
            MeshRenderer temp = GetComponent<MeshRenderer>();

            if (temp != null)
            {
                MeshRenderers.Add(temp);
                colors.Add(temp.material.color);
                materials.Add(temp.material);
            }

            foreach (Transform t in transform)
            {
                temp = t.GetComponent<MeshRenderer>();

                if (temp != null)
                {
                    MeshRenderers.Add(temp);
                    colors.Add(temp.material.color);
                    materials.Add(temp.material);
                }
            }
        }
    }

    private bool highlight;
    public void ToggleHighLight()
    {
        if (highlight)
        {
            for (int i = 0; i < MeshRenderers.Count; i++)
            {
                MeshRenderers[i].material = materials[i];
                MeshRenderers[i].material.color = colors[i];
            }

        }
        else
        {
            for (int i = 0; i < MeshRenderers.Count; i++)
            {
                MeshRenderers[i].material = highlightMaterial;
                MeshRenderers[i].material.color = UtilClass.ChangeColorBrightness(colors[i], brightness);
            }
        }
        highlight = !highlight;
    }
}