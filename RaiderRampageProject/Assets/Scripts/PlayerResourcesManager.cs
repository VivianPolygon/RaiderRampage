using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourcesManager : MonoBehaviour
{
    public static PlayerResourcesManager instance;
    //current quantity of ammo
    [Header("Ammo Quantity")]
    public int pistolAmmo;
    public int shotGunAmmo;
    public int machineGunAmmo;
    public int rocketLauncherAmmo;
    //regen of each ammo type per second
    [Header("Ammo Regen Per Second")]
    public float pistolAmmoRegen;
    public float shotGunAmmoRegen;
    public float machineGunAmmoRegen;
    public float rocketLauncherAmmoRegen;
    //maximum amout of ammo
    [Header("Ammo Max")]
    public int pistolAmmoMax;
    public int shotGunAmmoMax;
    public int machineGunAmmoMax;
    public int rocketLauncherAmmoMax;

    //used to caculate current ammo, as a float
    [HideInInspector]
    public float pistolAmmoCalc;
    [HideInInspector]
    public float shotGunAmmoCalc;
    [HideInInspector]
    public float machineGunAmmoCalc;
    [HideInInspector]
    public float rocketLauncherAmmoCalc;

    //used to check if resources should be regenerated over time or not
    public enum resourcesGamestate
    {
        active, 
        merge
    }

    public resourcesGamestate state;

    //established a singleton 
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

    private void Start()
    {
        //sets the values added for caculations to initial ammo amount
        pistolAmmoCalc = pistolAmmo;
        shotGunAmmoCalc = shotGunAmmo;
        machineGunAmmoCalc = machineGunAmmo;
        rocketLauncherAmmoCalc = rocketLauncherAmmo;
    }

    private void Update()
    {
        switch (state)
        {
            case resourcesGamestate.active:
                RegenAmmo();
                break;
            case resourcesGamestate.merge:
                break;
            default:
                break;
        }
    }

    private void RegenAmmo()
    {
        //caculates the current amount of ammo from the regen and caps it to the respective ammo max, and prevents negative values
        pistolAmmoCalc = Mathf.Clamp(pistolAmmoCalc + (pistolAmmoRegen * Time.deltaTime), 0, pistolAmmoMax);
        shotGunAmmoCalc = Mathf.Clamp(shotGunAmmoCalc + (shotGunAmmoRegen * Time.deltaTime), 0, shotGunAmmoMax);
        machineGunAmmoCalc = Mathf.Clamp(machineGunAmmoCalc + (machineGunAmmoRegen * Time.deltaTime), 0, machineGunAmmoMax);
        rocketLauncherAmmoCalc = Mathf.Clamp(rocketLauncherAmmoCalc + (rocketLauncherAmmoRegen * Time.deltaTime), 0, rocketLauncherAmmoMax);
        //rounds the cauclated ammo to a float and applies it

        pistolAmmo = Mathf.RoundToInt(pistolAmmoCalc);
        shotGunAmmo = Mathf.RoundToInt(shotGunAmmoCalc);
        machineGunAmmo = Mathf.RoundToInt(machineGunAmmoCalc);
        rocketLauncherAmmo = Mathf.RoundToInt(rocketLauncherAmmoCalc);
    }

}
