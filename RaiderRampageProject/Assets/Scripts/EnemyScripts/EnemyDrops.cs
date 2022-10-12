using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [Header("Does this Enemy Drop Barrels?")]
    [SerializeField] private bool dropsBarrels = true;

    [Header("Chance to Drop a Barrel")]
    [SerializeField] private int barrelDropChance;

    [Header("Chance for each Barrel to Drop Relative to Eachother")]
    [SerializeField] private int SMGDropWeight;
    [SerializeField] private int pistolDropWeight;
    [SerializeField] private int shotGunDropWeight;
    [SerializeField] private int machineGunDropWeight;
    [SerializeField] private int sniperDropWeight;
    [SerializeField] private int rocketLauncherDropWeight;
    //array used for looping through in probability checks
    private int[] barrelWeights;


    [Header("Does This Enemy Drop Scrap?")]
    [SerializeField] private bool dropsScrap;

    [Header("How Much Scrap?")]
    [SerializeField] private int droppedScrapMin;
    [SerializeField] private int droppedScrapMax;


    void Awake()
    {
        //sets the weitghts array appropiatly
        barrelWeights = new int[6];
        barrelWeights[0] = SMGDropWeight;
        barrelWeights[1] = pistolDropWeight;
        barrelWeights[2] = shotGunDropWeight;
        barrelWeights[3] = machineGunDropWeight;
        barrelWeights[4] = sniperDropWeight;
        barrelWeights[5] = rocketLauncherDropWeight;
        //clamps the percent
        barrelDropChance = Mathf.Clamp(barrelDropChance, 0, 100);
    }

    private void DropBarrel()
    {
        //prevents barrel drops if inventory is full
        if(!PlayerResourcesManager.instance.CheckInventoryFull())
        {
            //determins if a barrel drops from overall chance
            if(Random.Range(1, 101) <= barrelDropChance)
            {
                //caculates weights and which barrel will drop
                int totalWeights = 0;
                for (int i = 0; i < barrelWeights.Length; i++)
                {
                    totalWeights += barrelWeights[i];
                }
                int selectedValue = Random.Range(1, totalWeights + 1);
                int trackedWeights = 0;
                for (int i = 0; i < barrelWeights.Length; i++)
                {
                    if (trackedWeights >= selectedValue)
                    {
                        PlayerResourcesManager.instance.AddBarrelToInventory(i - 1, 0);
                        return;
                    }
                    else
                    {
                        trackedWeights += barrelWeights[i];
                    }
                }
            }
        }
    }

    private void DropScrap()
    {
        PlayerResourcesManager.scrap += Random.Range(droppedScrapMin, droppedScrapMax + 1);
        UIEvents.instance.UpdateScrapCounts();
    }

    private void OnDestroy()
    {
        if(dropsBarrels)
        {
            DropBarrel();
        }

        if(dropsScrap)
        {
            DropScrap();
        }
    }
}
