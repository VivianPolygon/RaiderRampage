using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script for the gunslots on the actual gun
public class GunSlot : MonoBehaviour
{
    //object for the spawned barrel prefab
    private GameObject slotBarrel;
    //position in the array, set up through the inspector in StaticGunData
    private int arrayPosition;

    //gets the position in the array from StaticGunData
    private void Awake()
    {
        arrayPosition = System.Array.IndexOf(StaticGunData.instance.gunSlots, this);
    }

    //updates barrel on the gun from the UI based on the currently active gunhead inputed
    //array of gunheads located on StaticGunData
    //function called from GunData
    public void UpdateSlotBarrel(int headNum)
    {
        //destroys current barrel
        if(slotBarrel != null)
        {
            Destroy(slotBarrel);
        }
        //checks for addon slots, if they are unlocked, renderes the slot's mesh independent of if there is a barrel or not
        if(StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].gameObject.TryGetComponent(out WorkshopExtraSlotLock slotLock))
        {
            if (TryGetComponent(out MeshRenderer renderer))
            {
                if (slotLock.locked && renderer != null)
                {
                    renderer.enabled = false;
                }
                else if (!slotLock.locked && renderer != null)
                {
                    renderer.enabled = true;
                }
            }
        }

        //updates dependent on enum data from the slots from the UI
        switch (StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotType)
            {               
            case BarrelType.SMG:
                slotBarrel = Instantiate(StaticGunData.instance.SMGPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                   
                slotBarrel.transform.parent = transform;

                if ((int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier > 2) //warning that makes sure all barrels have tiers
                { Debug.LogWarning("Barrel In inventory is unteired and will cause errors, make sure initial barrels are all set to have tiers"); }
                break;
               
            case BarrelType.Pistol:                   
                slotBarrel = Instantiate(StaticGunData.instance.PistolPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                  
                slotBarrel.transform.parent = transform;

                if ((int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier > 2) //warning that makes sure all barrels have tiers
                { Debug.LogWarning("Barrel In inventory is unteired and will cause errors, make sure initial barrels are all set to have tiers"); }
                break;
               
            case BarrelType.Shotgun:                 
                slotBarrel = Instantiate(StaticGunData.instance.ShotGunPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                 
                slotBarrel.transform.parent = transform;

                if ((int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier > 2) //warning that makes sure all barrels have tiers
                { Debug.LogWarning("Barrel In inventory is unteired and will cause errors, make sure initial barrels are all set to have tiers"); }
                break;
               
            case BarrelType.MachineGun:                  
                slotBarrel = Instantiate(StaticGunData.instance.MachineGunPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                 
                slotBarrel.transform.parent = transform;

                if ((int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier > 2) //warning that makes sure all barrels have tiers
                { Debug.LogWarning("Barrel In inventory is unteired and will cause errors, make sure initial barrels are all set to have tiers"); }
                break;
                
            case BarrelType.Sniper:                  
                slotBarrel = Instantiate(StaticGunData.instance.SniperPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                   
                slotBarrel.transform.parent = transform;

                if ((int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier > 2) //warning that makes sure all barrels have tiers
                { Debug.LogWarning("Barrel In inventory is unteired and will cause errors, make sure initial barrels are all set to have tiers"); }
                break;
              
            case BarrelType.RocketLauncher:                  
                slotBarrel = Instantiate(StaticGunData.instance.RocketLauncherPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                  
                slotBarrel.transform.parent = transform;

                if ((int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier > 2) //warning that makes sure all barrels have tiers
                { Debug.LogWarning("Barrel In inventory is unteired and will cause errors, make sure initial barrels are all set to have tiers"); }
                break;
                
            case BarrelType.Empty:                
                break;             
            default:                 
                break;
            }
    }
}
