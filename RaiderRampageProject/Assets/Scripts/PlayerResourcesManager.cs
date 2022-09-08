using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that tracks player resources like ammo and scrap
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

    //max clip sizes
    [Header("Clip Max")]
    public int pistolClipMax;
    public int shotGunClipMax;
    public int machineGunClipMax;
    public int rocketLauncherClipMax;
    //current amount in clip
    [Header("Clip Current")]
    public int pistolClipCurrent;
    public int shotGunClipCurrent;
    public int machineGunClipCurrent;
    public int rocketLauncherClipCurrent;

    //used to caculate current ammo, as a float
    [HideInInspector]
    public float pistolAmmoCalc;
    [HideInInspector]
    public float shotGunAmmoCalc;
    [HideInInspector]
    public float machineGunAmmoCalc;
    [HideInInspector]
    public float rocketLauncherAmmoCalc;

    private int reloadValue;
    private Coroutine reloadCoroutine = null;

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

        //updates the ammo sliders
        UIData.instance.UpdateAmmoSlider(0, pistolAmmoMax, pistolAmmo);
        UIData.instance.UpdateAmmoSlider(1, machineGunAmmoMax, machineGunAmmo);
        UIData.instance.UpdateAmmoSlider(2, shotGunAmmoMax, shotGunAmmo);
        UIData.instance.UpdateAmmoSlider(3, rocketLauncherAmmoMax, rocketLauncherAmmo);
    }

    public void Reload()
    {
        if(reloadCoroutine == null)
        {
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        GunData.instance.reloading = true;
        for (float i = 0; i < GunData.instance.reloadTime; i += Time.deltaTime)
        {
            UIData.instance.ammoReloadTimerSlider.value = i / GunData.instance.reloadTime;
            yield return null;
        }
        //reloads pistol ammo
        reloadValue = Mathf.Clamp(Mathf.Clamp(pistolClipMax, 0, pistolClipMax - pistolClipCurrent), 0, pistolAmmo);
        pistolAmmoCalc -= reloadValue;
        pistolClipCurrent += reloadValue;

        //reloads machinegun ammo
        reloadValue = Mathf.Clamp(Mathf.Clamp(machineGunClipMax, 0, machineGunClipMax - machineGunClipCurrent), 0, machineGunAmmo);
        machineGunAmmoCalc -= reloadValue;
        machineGunClipCurrent += reloadValue;

        //reloads shotgun ammo
        reloadValue = Mathf.Clamp(Mathf.Clamp(shotGunClipMax, 0, shotGunClipMax - shotGunClipCurrent), 0, shotGunAmmo);
        shotGunAmmoCalc -= reloadValue;
        shotGunClipCurrent += reloadValue;

        //reloads rockerlauncher ammo
        reloadValue = Mathf.Clamp(Mathf.Clamp(rocketLauncherClipMax, 0, rocketLauncherClipMax - rocketLauncherClipCurrent), 0, rocketLauncherAmmo);
        rocketLauncherAmmoCalc -= reloadValue;
        rocketLauncherClipCurrent += reloadValue;


        //updates clipsliders
        UIData.instance.UpdateAmmoSlider(4, pistolClipMax, pistolClipCurrent);
        UIData.instance.UpdateAmmoSlider(5, machineGunClipMax, machineGunClipCurrent);
        UIData.instance.UpdateAmmoSlider(6, shotGunClipMax, shotGunClipCurrent);
        UIData.instance.UpdateAmmoSlider(7, rocketLauncherClipMax, rocketLauncherClipCurrent);


        UIData.instance.ammoReloadTimerSlider.value = 1;
        GunData.instance.reloading = false;
        reloadCoroutine = null;
    }

}
