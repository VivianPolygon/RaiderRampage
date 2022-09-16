using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script used for holding sets of slots on the UI Gunheads
public class GunHeadUI : MonoBehaviour
{
    //array of the gunhead's gunslots
    public WorkshopBarrelSlot[] headSlots;

    void Start()
    {
        //initilizes the count
        UpdateHeadSlotCount();
    }

    //counts the slots on the gun using trygetcomponent, warns if a childed object is not a barrelslot, and destroys it on runtime, then adds the slots to the headSlots array
    public void UpdateHeadSlotCount()
    {
        int slots = 0;
        foreach(Transform child in transform)
        {
            if (child.TryGetComponent(out WorkshopBarrelSlot slot))
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
