using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSlot : MonoBehaviour
{
    private GameObject slotBarrel;
    private int arrayPosition;

    private void Start()
    {
        arrayPosition = System.Array.IndexOf(StaticGunData.instance.gunSlots, this);
    }

    public void UpdateSlotBarrel(int headNum)
    {
        if(slotBarrel != null)
        {
            Destroy(slotBarrel);
        }

            
        switch (StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotType)
            {               
            case BarrelType.SMG:                   
                slotBarrel = Instantiate(StaticGunData.instance.SMGPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                   
                slotBarrel.transform.parent = transform;                    
                break;
               
            case BarrelType.Pistol:                   
                slotBarrel = Instantiate(StaticGunData.instance.PistolPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                  
                slotBarrel.transform.parent = transform;                
                break;
               
            case BarrelType.Shotgun:                 
                slotBarrel = Instantiate(StaticGunData.instance.ShotGunPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                 
                slotBarrel.transform.parent = transform;                  
                break;
               
            case BarrelType.MachineGun:                  
                slotBarrel = Instantiate(StaticGunData.instance.MachineGunPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                 
                slotBarrel.transform.parent = transform;                  
                break;
                
            case BarrelType.Sniper:                  
                slotBarrel = Instantiate(StaticGunData.instance.SniperPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                   
                slotBarrel.transform.parent = transform;                 
                break;
              
            case BarrelType.RocketLauncher:                  
                slotBarrel = Instantiate(StaticGunData.instance.RocketLauncherPrefabs[(int)StaticGunData.instance.workshopGunHeads[headNum].headSlots[arrayPosition].slotTier], transform.position, transform.rotation);                  
                slotBarrel.transform.parent = transform;                
                break;
                
            case BarrelType.Empty:                
                break;             
            default:                 
                break;
            }
    }
}
