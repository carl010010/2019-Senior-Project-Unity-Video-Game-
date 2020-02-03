using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent currentItemChanged;


    public int Value = 0;
    [HideInInspector]
    public List<Item> items = new List<Item>();

    [HideInInspector]
    public Item Gold = null;

    [SerializeField]
    int _currentItem = 0;
    int currentItem
    {
        get
        {
            return _currentItem;
        }
        set
        {
            _currentItem = value;
            currentItemChanged.Invoke();
        }
    }

    public float m_MaxDistanceItemCheck = 5;
    public float m_ItemCheckradius = 1;
    private List<Item> hitObjects = new List<Item>();
    private Camera m_Camera;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    void FixedUpdate()
    {
        LookForItems();
    }

    [SerializeField]
    private Item Highlighted;

    private void Update()
    {
        if (hitObjects.Count > 0)
        {
            if (Highlighted == null)
            {
                Highlighted = hitObjects[hitObjects.Count - 1];
                Highlighted.ToggleHighLight();
            }
            else if (Highlighted != hitObjects[hitObjects.Count - 1])
            {
                //Turn old item highlight off
                Highlighted.ToggleHighLight();
                Highlighted = hitObjects[hitObjects.Count - 1];
                //Turn new item highlight on
                Highlighted.ToggleHighLight();
            }

            if (Input.GetMouseButtonDown(1))
            {
                //Turn The highlight off
                Highlighted.ToggleHighLight();
                Highlighted = null;
                AddItem(hitObjects[hitObjects.Count - 1]);
                hitObjects[hitObjects.Count - 1].gameObject.SetActive(false);

            }
        }
        else if (Highlighted != null)
        {
            Highlighted.ToggleHighLight();
            Highlighted = null;
        }
    }

    public void AddItem(Item item)
    {
        item.Pickup();

        if (item.itemType == Item.ItemType.KEY && items.Any(x => x.itemType == Item.ItemType.KEY && x.Value == item.Value))
        {
            return;
        }
        else if (item.itemType == Item.ItemType.GOLD)
        {
            if (Gold == null)
            {
                Gold = item;
                items.Add(item);
            }
            else
            {
                Gold.Value += item.Value;
            }
        }
        else if (item.itemType == Item.ItemType.VALUABLE)
        {
            items.Add(item);
            Value += item.Value;
        }
        else
        {
            items.Add(item);
        }

        items.Sort(item.Compare);

    }

    private void LookForItems()
    {
        hitObjects.Clear();
        RaycastHit[] hits = Physics.SphereCastAll(m_Camera.transform.position, m_ItemCheckradius,
                                                  m_Camera.transform.forward, m_MaxDistanceItemCheck, 1 << LayerConstants.Loot);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {

                Item item = hit.transform.GetComponent<Item>();
                if (item != null && !Physics.Linecast(hit.transform.position, m_Camera.transform.position))
                {
                    hitObjects.Add(item);
                }
            }
        }
    }

    public void IncrementCurrentItem()
    {
        if (items.Count == 0)
            return;

        currentItem = (currentItem + 1) % items.Count;
    }

    public void DecrementCurrentItem()
    {
        if (items.Count == 0)
            return;

        if (currentItem <= 0)
        {
            currentItem = items.Count - 1;
        }
        else
        {
            currentItem--;
        }
    }

    public int GetGold()
    {
        if (Gold == null)
            return 0;
        else
            return Gold.Value;
    }

    public Item GetCurrentItem()
    {
        return items.Count == 0 ? null : items[currentItem];
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (hitObjects.Count > 0)
        {
            foreach (Item item in hitObjects)
            {
                Gizmos.DrawWireCube(item.transform.position,
                                    (item.GetComponent<Collider>().bounds.extents * 2) * 1.2f);
            }
        }
    }
}
