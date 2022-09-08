using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Script that holds UI elements and their data
public class UIData : MonoBehaviour
{
    //singleton instance
    public static UIData instance;

    //sliders for the ammo bars and fills
    [SerializeField]
    private Slider[] ammoSliders;
    [SerializeField]
    private Image[] fillImages;

    [SerializeField]
    public Slider ammoReloadTimerSlider;

    //colors for the bar fills
    [SerializeField] 
    private Color sliderEmptyColor;
    [SerializeField]
    private Color sliderFullColor;

    //establishes the singleton
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
        //initilizes the sliders
        SetAllAmmoSliders();
    }

    //updates 1 ammo slider based on input
    public void UpdateAmmoSlider(int slidernum, int maxCapacity, int currentCapacity)
    {
        ammoSliders[slidernum].value = currentCapacity / (float)maxCapacity;
        fillImages[slidernum].color = Color.Lerp(sliderEmptyColor, sliderFullColor, currentCapacity / (float)maxCapacity);
    }
    //function that can be used to set all ammo sliders at once
    public void SetAllAmmoSliders()
    {
        //ammo sliders
        UpdateAmmoSlider(0, PlayerResourcesManager.instance.pistolAmmoMax, PlayerResourcesManager.instance.pistolAmmo);
        UpdateAmmoSlider(1, PlayerResourcesManager.instance.machineGunAmmoMax, PlayerResourcesManager.instance.machineGunAmmo);
        UpdateAmmoSlider(2, PlayerResourcesManager.instance.shotGunAmmoMax, PlayerResourcesManager.instance.shotGunAmmo);
        UpdateAmmoSlider(3, PlayerResourcesManager.instance.rocketLauncherAmmoMax, PlayerResourcesManager.instance.rocketLauncherAmmo);
        //clip sliders
        UpdateAmmoSlider(4, PlayerResourcesManager.instance.pistolClipMax, PlayerResourcesManager.instance.pistolClipCurrent);
        UpdateAmmoSlider(5, PlayerResourcesManager.instance.machineGunClipMax, PlayerResourcesManager.instance.machineGunClipCurrent);
        UpdateAmmoSlider(6, PlayerResourcesManager.instance.shotGunClipMax, PlayerResourcesManager.instance.shotGunClipCurrent);
        UpdateAmmoSlider(7, PlayerResourcesManager.instance.rocketLauncherClipMax, PlayerResourcesManager.instance.rocketLauncherClipCurrent);
    }
}
