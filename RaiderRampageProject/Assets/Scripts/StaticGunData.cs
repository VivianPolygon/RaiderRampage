using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//script that holds data for the gun that will not need to change on runtime
public class StaticGunData : MonoBehaviour
{
    public static StaticGunData instance;


    [Header("Barrel Priority")]
    //number for the corresponding cursor sprite for that barel type, displayed if its the majority
    public int SMGSpritePriorityAndNumber;
    public int pistolSpritePriorityAndNumber;
    public int machineGunSpritePriorityAndNumber;
    public int shotgunSpritePriorityAndNumber;
    public int sniperSpritePriorityAndNumber;
    public int rocketLauncherSpritePriorityAndNumber;

    [Header("The Four Gunhead Loadouts from the UI")]
    public GunHeadUI[] workshopGunHeads;
    [Header("The Slots for Barrrels on the Actual Gun")]
    public GunSlot[] gunSlots;

    [Header("Prefabs for each guntype in teir order")]
    public GameObject[] SMGPrefabs;
    public GameObject[] PistolPrefabs;
    public GameObject[] MachineGunPrefabs;
    public GameObject[] ShotGunPrefabs;
    public GameObject[] SniperPrefabs;
    public GameObject[] RocketLauncherPrefabs;

    private void Awake()
    {

        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

    }
}