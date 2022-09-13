using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHeadUI : MonoBehaviour
{
    public WorkshopBarrelSlot[] headSlots;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHeadSlotCount();
    }


    public void UpdateHeadSlotCount()
    {
        int slots = 0;
        foreach(Transform child in transform)
        {
            if (child.TryGetComponent<WorkshopBarrelSlot>(out WorkshopBarrelSlot slot))
            {
                slots++;
            }
            else
            {
                Debug.LogWarning("there is a gameobject in the gunhead UI that does not have the WorkShopBarrelSlot script, please remove it, it has been removed on runtime");
                Destroy(child.gameObject);
            }
        }

        headSlots = new WorkshopBarrelSlot[slots];

        for (int i = 0; i < headSlots.Length; i++)
        {
            headSlots[i] = transform.GetChild(i).GetComponent<WorkshopBarrelSlot>();
        }
    }
}
