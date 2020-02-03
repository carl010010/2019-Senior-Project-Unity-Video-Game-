using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Util;

public class Item : MonoBehaviour, IComparer<Item> {

    public enum ItemType {GOLD, VALUABLE, KEY, WEAPON, LOCKPICK};

    public ItemType itemType = ItemType.GOLD;
    public int Value = 0;

    public UnityEvent OnPickUp;

    [Space]
    public Material highlightMaterial;
    [Range(0,1)]
    public float brightness = 0.4f;
    private List<MeshRenderer> MeshRenderers = new List<MeshRenderer>();
    private List<Color> colors = new List<Color>();
    private List<Material> materials = new List<Material>();


    void Start()
    {
        if (highlightMaterial == null)
        {
            Debug.LogError("No highlightMaterial found on " + name, this);
        }

        if (MeshRenderers.Count == 0)
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

    public virtual void Use()
    { }

    //Used for List sorting
    public int Compare(Item a, Item b)
    {
        if ((a.itemType == b.itemType))
            return 0;
        if (a.itemType == ItemType.LOCKPICK && (b.itemType == ItemType.GOLD || b.itemType == ItemType.KEY || b.itemType == ItemType.VALUABLE || b.itemType == ItemType.WEAPON) 
            || a.itemType == ItemType.KEY && (b.itemType == ItemType.GOLD || b.itemType == ItemType.VALUABLE || b.itemType == ItemType.WEAPON)
            || a.itemType == ItemType.WEAPON && (b.itemType == ItemType.GOLD || b.itemType == ItemType.VALUABLE)
            || a.itemType == ItemType.VALUABLE && b.itemType == ItemType.GOLD
            )
            return 1;

        return -1;
    }

    public void Pickup()
    {
        OnPickUp.Invoke();
    }

    private bool highlight;
    public void ToggleHighLight()
    {
        if(highlight)
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
                MeshRenderers[i].material.color = UtilClass.ChangeColorBrightness(colors[i], brightness);//colors[i] + new Color(0.3f,0.3f,0.3f);
            }
        }
        highlight = !highlight;
    }

}
