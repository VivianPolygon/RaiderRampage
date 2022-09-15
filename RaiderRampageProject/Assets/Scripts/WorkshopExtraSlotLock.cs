using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopExtraSlotLock : MonoBehaviour
{
    //script that locks its coresponding slot's, used for slots only acessible by upgrade, requires a workshopbarrelslot script to be on the same object

    [HideInInspector]
    public bool locked;

    private WorkshopBarrelSlot workshopBarrelSlot;



    void Start()
    {
        if (TryGetComponent(out WorkshopBarrelSlot slotscript))
        {
            workshopBarrelSlot = slotscript;
        }
        else
        {
            Debug.LogWarning("an object has the WorkshopExtraSlotLock script without also having a workshopbarrelSlot on it, object name: " + name);
        }

        //initilizes to false, so locking runs properly, locks initialy
        locked = false;
        Locked(true);
    }


    public void Locked(bool lockState)
    {
        //returns out if the funtion would be redundant
        if(locked == lockState)
        {
            return;
        }
        //sets locked to the state set if its diffrent
        locked = lockState;

        //updates stuff
        if(locked)
        {
            workshopBarrelSlot.enabled = false;

            return;
        }
        if(!locked)
        {


            return;
        }
    }
}
