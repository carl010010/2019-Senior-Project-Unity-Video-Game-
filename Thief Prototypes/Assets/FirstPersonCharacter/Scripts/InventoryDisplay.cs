using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public Inventory inventory;

    public float TimeDisplayInventory = 5;
    private float Timer;
    private bool InventoryForcedOpen;

    [Space]
    public float RotateSpeed = 5;
    public Vector3 objectPos;
    GameObject displayObject;


    private void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("Did you for get to plug in the inventory", this);
        }
        else
        {
            inventory.currentItemChanged.AddListener(ItemChanged);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (displayObject != null)
        {
            if (!InventoryForcedOpen && displayObject.activeInHierarchy && Timer <= 0)
            {
                displayObject.SetActive(false);
            }
            else if (Timer > 0)
            {
                Timer -= Time.deltaTime;
            }
            //displayObject.transform.position = objectPos;
            displayObject.transform.Rotate(0, RotateSpeed * Time.deltaTime, 0, Space.World);
        }
    }

    public bool IsInventoryOpen()
    {
        if (displayObject == null)
            return false;

        return displayObject.activeInHierarchy;
    }

    void ItemChanged()
    {
        if (!InventoryForcedOpen)
        {
            Timer = TimeDisplayInventory;
        }

        CreateObject();
        displayObject.SetActive(true);
    }

    private void CreateObject()
    {
        Item currentItem = inventory.GetCurrentItem();
        //HACK if two items are the same type i.e. both valuables but are different with the same Value, one of the items might not show in inventory
        if (displayObject == null || (displayObject.GetComponent<Item>().itemType != currentItem.itemType || displayObject.GetComponent<Item>().Value != currentItem.Value))
        {
            Destroy(displayObject);

            if (currentItem == null)
                return;

            displayObject = Instantiate(currentItem.gameObject);
            displayObject.transform.position = objectPos;
            displayObject.layer = LayerConstants.Veiwer;
            foreach (Transform g in displayObject.transform)
            {
                g.gameObject.layer = LayerConstants.Veiwer;
            }
        }
    }

    public void ToggleInventory()
    {
        if (displayObject == null)
            CreateObject();

        if (displayObject != null)
        {

            if (displayObject.activeInHierarchy)
            {
                InventoryForcedOpen = false;
            }
            else
            {
                InventoryForcedOpen = true;
            }
            Timer = 0;
            displayObject.SetActive(!displayObject.activeInHierarchy);
        }
    }
}
