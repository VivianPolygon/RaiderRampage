using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Script that locks its coresponding slot's, used for slots only acessible by upgrade, requires a workshopbarrelslot script to be on the same object
public class WorkshopExtraSlotLock : MonoBehaviour
{
    //bool to determine if the slot is available or not
    [HideInInspector]
    public bool locked;
    //image for the slot background
    [SerializeField]
    private Image extraSlotBackgroundImage;
    //workshopbarrelsot script thats on the same object
    private WorkshopBarrelSlot workshopBarrelSlot;
    //colors used to modify the background slot image depending if its active or not
    static private Color activeColor;
    static private Color inactiveColor;

    void Start()
    {
        //trys to get the workshopbarrelslot, if it cant logs a warning
        if (TryGetComponent(out WorkshopBarrelSlot slotscript))
        {
            workshopBarrelSlot = slotscript;
        }
        else
        {
            Debug.LogWarning("an object has the WorkshopExtraSlotLock script without also having a workshopbarrelSlot on it, object name: " + name);
        }

        //initilizes the colors, modifies only transparency depending on if active
        activeColor = new Color(1, 1, 1, 1);
        inactiveColor = new Color(1, 1, 1, 0.25f);

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

        //updates the bool and the color
        if(locked)
        {
            workshopBarrelSlot.enabled = false;
            extraSlotBackgroundImage.color = inactiveColor;
            return;
        }
        if(!locked)
        {
            workshopBarrelSlot.enabled = true;
            extraSlotBackgroundImage.color = activeColor;
            return;
        }
    }
}
