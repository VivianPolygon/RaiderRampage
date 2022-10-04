using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that tracks player resources like ammo and scrap
public class PlayerResourcesManager : MonoBehaviour
{
    public static bool ammoRegen;

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

    //used to determine how much ammo is reloaded, needed for if the ammount in ammo storage is less than the clip capacity
    private int reloadValue;
    //coroutine used for reloading
    private Coroutine reloadCoroutine = null;

    //used to check if ammo resources should be regenerated over time or not

    [Header("Holds the Inventory Slots That Store Unused Barrels")]
    [SerializeField]
    private Transform inactiveBarrelInventoryParentObject;

    private WorkshopBarrelSlot[] inactiveInventoryBarrelSlots;

    [Header("Upgrade Resources")]
    //initial scrap amount and tracked quantity of scrap
    [SerializeField]
    private int startingScrap;
    public static int scrap;
    //initial addon quantity
    public int startingSlotAddonQuantity;

    [Header("Upgrade Resources Caps")]
    public int scrapCap;
    public int addonCap;

    [Header("Upgrade Resources Costs")]
    public int addonCost;


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

        scrap = startingScrap;
    }

    private void Update()
    {
        //regens ammo based on delta time and regen speeds if the flag for ammo regen is true
        if(ammoRegen)
        {
            RegenAmmo();
        }
    }


    private void Start()
    {
        //sets the values added for caculations to initial ammo amount
        pistolAmmoCalc = pistolAmmo;
        shotGunAmmoCalc = shotGunAmmo;
        machineGunAmmoCalc = machineGunAmmo;
        rocketLauncherAmmoCalc = rocketLauncherAmmo;

        //initilizes the merging inventory on the excess inventory slots
        InitilizeInventory();
    }


    public void RegenAmmo()
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
        //reloads the clip, if the coroutine is still running, it wont run
        if(reloadCoroutine == null)
        {
            UIData.instance.reloadButtonImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.activeReloadButtonSpriteNumber);
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

        //sets the bar to full
        UIData.instance.ammoReloadTimerSlider.value = 1;
        //sets the flag for reloading to false (used to prevent shooting while reloading)
        GunData.instance.reloading = false;
        //sets the coroutine to null so it can be ran again
        reloadCoroutine = null;
        //sets the sprite back to default
        UIData.instance.reloadButtonImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.inactiveReloadButtonSpriteNumber);
    }

    //initilizes the inactive barrel inventory (slots that are for storage and aren't on any gunheads)
    private void InitilizeInventory()
    {
        //used to safly track each slot that has a WorkshopBarrelSlot script
        int inventorySlotQuantity = 0;

        //loops through the children on the parent object, should each be a slot, will warn and destroy children that dont have a WorkshopBarrelSlot script
        foreach (Transform child in inactiveBarrelInventoryParentObject)
        {
            if (child.TryGetComponent(out WorkshopBarrelSlot slot))
            {
                inventorySlotQuantity++;
            }
            else
            {
                Debug.LogWarning("there is an object in the inactivebarrleinventory that is not a barrel slot, please remove it, it has been destroyed on runtime");
                Destroy(child);
            }
        }

        //establishes the array for the slots, used to add a barrel to the array
        inactiveInventoryBarrelSlots = new WorkshopBarrelSlot[inventorySlotQuantity];
        for (int i = 0; i < inactiveInventoryBarrelSlots.Length; i++)
        {
            inactiveInventoryBarrelSlots[i] = inactiveBarrelInventoryParentObject.GetChild(i).GetComponent<WorkshopBarrelSlot>();
        }
    }

    //adds to the first slot of the inactive inventory array (slots that are for storage and aren't on any gunheads)
    public void AddBarrelToInventory(int barrelNumber, int barrelTier)
    {
        //prevents tiers that dont exsist being added
        barrelTier = Mathf.Clamp(barrelTier, 0, 2);
        barrelNumber = Mathf.Clamp(barrelNumber, 0, 5);

        //loops through inventory, adds barrel from inputs to the first slot available, the returns out
        foreach(WorkshopBarrelSlot slot in inactiveInventoryBarrelSlots)
        {
            if(slot.slotType == BarrelType.Empty)
            {
                slot.slotType = (BarrelType)barrelNumber;
                slot.slotTier = (BarrelTeir)barrelTier;

                slot.UpdateDisplay();

                return;
            }
        }
    }

    //Delete once tested, and above is implemented in normal gameplay, only needed for debugging
    //same function as abouve, however teir is defaulted to 1, buttons in unity can only have one parameter
    public void AddBarrelToInventoryButton(int barrelNumber)
    {
        //prevents tiers that dont exsist being added
        barrelNumber = Mathf.Clamp(barrelNumber, 0, 5);

        //loops through inventory, adds barrel from inputs to the first slot available, the returns out
        foreach (WorkshopBarrelSlot slot in inactiveInventoryBarrelSlots)
        {
            if (slot.slotType == BarrelType.Empty)
            {
                slot.slotType = (BarrelType)barrelNumber;
                slot.slotTier = (BarrelTeir)0;

                slot.UpdateDisplay();

                return;
            }
        }
    }

    public void PurchaseAddon()
    {
        if(scrap >= addonCost && SlotAddonInventory.slotAddonQuantity + SlotAddonInventory.currentAddonEquipt < addonCap)
        {
            scrap -= addonCost;
            SlotAddonInventory.instance.AddSlotAddons(1);
            UIEvents.instance.UpdateScrapCounts();

            if(SlotAddonInventory.slotAddonQuantity + SlotAddonInventory.currentAddonEquipt >= addonCap)
            {
                UIData.instance.slotAddonButton.gameObject.SetActive(false);
            }
        }
    }
}
