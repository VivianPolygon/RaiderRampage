using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that tracks player resources like ammo and scrap, as well as houses functions needed for the upgrade screen
public class PlayerResourcesManager : MonoBehaviour
{
    public static bool ammoRegen;

    public static PlayerResourcesManager instance;

    //used to caculate current ammo, as a float
    [HideInInspector] public float[] ammoRegenCalcs;

    //regen of each ammo type per second
    [Header("Ammo Regen Rates")]
    public float pistolAmmoRegen;
    public float shotGunAmmoRegen;
    public float machineGunAmmoRegen;
    public float rocketLauncherAmmoRegen;

    [HideInInspector] public float[] ammoRegenRates;

    //maximum amout of ammo
    [HideInInspector] public int[] ammoMaxes;
    //max clip sizes
    [HideInInspector] public int[] clipMaxes;

    //current quantity of ammo
    [HideInInspector] public int[] ammoQuantities;
    //current quantity of ammo in clip
    [HideInInspector] public int[] clipQuantities;

    //number of icons for clip
    [Header("ClipIcons")]
    public int pistolClipIcons;
    public int shotGunClipIcons;
    public int machineGunClipIcons;
    public int rockerLauncherClipIcons;

    [HideInInspector] public int[] clipIconAmount;

    //number of icons for ammo
    [Header("AmmoIcons")]
    public int pistolAmmoIcons;
    public int shotGunAmmoIcons;
    public int machineGunAmmoIcons;
    public int rockerLauncherAmmoIcons;

    [HideInInspector] public int[] ammoIconAmount;

    //ammo in each icon
    [Header("Icon Value")]
    public int pistolIconQuantity;
    public int shotGunIconQuantity;
    public int machineGunIconQuantity;
    public int rockerLauncherIconQuantity;

    [HideInInspector] public int[] iconValues;


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

    [Header("Clip Icons Gained from Clip Upgrades")]
    [SerializeField] private int pistolClipIncrease;
    [SerializeField] private int shotGunClipIncrease;
    [SerializeField] private int machineGunClipIncrease;
    [SerializeField] private int rocketLauncherClipIncrease;

    [Header("Ammo Icons Gained from Clip Upgrades")]
    [SerializeField] private int pistolAmmoIncrease;
    [SerializeField] private int shotGunAmmoIncrease;
    [SerializeField] private int machineGunAmmoIncrease;
    [SerializeField] private int rocketLauncherAmmoIncrease;

    [Header("Multiplyer Increase Per Upgrade")]
    [SerializeField] private float damageMultIncrease;
    [SerializeField] private float fireSpeedMultIncrease;

    [Header("grenade refill delay time")]
    public float grenadeRefillTime;

    public static float damageMult = 1;
    public static float fireSpeedMult = 1;

    public static float murderScore; //used for Overdrive Gauge, currently adds an enemies damage when they die to increase it

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

        //Drains the murderscore from the overdrive gauge
        DrainMurderScore();
    }


    private void Start()
    {
        //initilizes the ammo arrays
        InitilizeAllAmmoArrays();

        //initilizes the merging inventory on the excess inventory slots
        InitilizeInventory();

        //initilizes multipliers to 1
        InitilizeMults();

    }


    private void InitilizeMults()
    {
        damageMult = 1;
        fireSpeedMult = 1;
    }

    public int MultiplyDamage(int damage)
    {
        return Mathf.RoundToInt(damage * damageMult);
    }

    public void DrainMurderScore()
    {
        if(murderScore > 0 && !OverdriveGauge.atTopTier && !OverdriveGauge.overdriveActive)
        {
            murderScore = Mathf.Clamp(murderScore - (OverdriveGauge.drainRate * Time.deltaTime), 0, OverdriveGauge.murderScoreMax);
            OverdriveGauge.UpdateFillCalculations();

        }
    }



    public void RegenAmmo()
    {
        for (int i = 0; i < ammoRegenCalcs.Length; i++)
        {
            //updates quantities
            ammoRegenCalcs[i] = Mathf.Clamp(ammoRegenCalcs[i] + (ammoRegenRates[i] * Time.deltaTime), 0, ammoMaxes[i]);
            ammoQuantities[i] = Mathf.RoundToInt(ammoRegenCalcs[i]);
            //updates ammobank drainIcons
            UIData.instance.UpdateAmmoDrainIcons();
        }
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

        //sets animator to reload
        GunData._gunAnim.SetBool("Reloading", true);

        //plays the reload audio
        GunData.instance.PlayReloadAudio();

        for (float i = 0; i < GunData.instance.reloadTime; i += Time.deltaTime)
        {
            UIData.instance.ammoReloadTimerSlider.value = i / GunData.instance.reloadTime;
            yield return null;
        }

        for (int i = 0; i < clipIconAmount.Length; i++)
        {
            //reloads each clip from respective ammo store
            reloadValue = Mathf.Clamp(Mathf.Clamp(clipMaxes[i], 0, clipMaxes[i] - clipQuantities[i]), 0, ammoQuantities[i]);
            ammoRegenCalcs[i] -= reloadValue;
            clipQuantities[i] += reloadValue;
            //updates UI DrainIcons
            UIData.instance.UpdateAllDrainIcons();
        }

        //sets the icon to empty, making it invisible
        UIData.instance.ammoReloadTimerSlider.value = 0;
        //sets the flag for reloading to false (used to prevent shooting while reloading)
        GunData.instance.reloading = false;
        //sets the coroutine to null so it can be ran again
        reloadCoroutine = null;
        //sets the sprite back to default
        UIData.instance.reloadButtonImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.inactiveReloadButtonSpriteNumber);
        //sets the animator back to normal
        GunData._gunAnim.SetBool("Reloading", false);
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

    //Delete once tested, and above is implemented in normal gameplay, only needed for debugging
    //same function as abouve, however teir is defaulted to 1, buttons in unity can only have one parameter
    public void CreateBarrel(int barrelNumber)
    {
        //prevents tiers that dont exsist being added
        barrelNumber = Mathf.Clamp(barrelNumber, 0, 5);

        //charges the player for their purchase, returns out if they dont have enough
        switch ((BarrelType)barrelNumber)
        {
            case BarrelType.SMG:
                if(!PriceCheckAndCharge(StaticGunData.instance.SMGPrefabs[0].GetComponent<GunBarrel>().purchasePrice))
                {
                    return;
                }
                break;
            case BarrelType.Pistol:
                if (!PriceCheckAndCharge(StaticGunData.instance.PistolPrefabs[0].GetComponent<GunBarrel>().purchasePrice))
                {
                    return;
                }
                break;
            case BarrelType.Shotgun:
                if (!PriceCheckAndCharge(StaticGunData.instance.ShotGunPrefabs[0].GetComponent<GunBarrel>().purchasePrice))
                {
                    return;
                }
                break;
            case BarrelType.MachineGun:
                if (!PriceCheckAndCharge(StaticGunData.instance.MachineGunPrefabs[0].GetComponent<GunBarrel>().purchasePrice))
                {
                    return;
                }
                break;
            case BarrelType.Sniper:
                if (!PriceCheckAndCharge(StaticGunData.instance.SniperPrefabs[0].GetComponent<GunBarrel>().purchasePrice))
                {
                    return;
                }
                break;
            case BarrelType.RocketLauncher:
                if (!PriceCheckAndCharge(StaticGunData.instance.RocketLauncherPrefabs[0].GetComponent<GunBarrel>().purchasePrice))
                {
                    return;
                }
                break;
            case BarrelType.Empty:
                break;
            default:
                break;
        }

        UIEvents.instance.UpdateScrapCounts();
        UIEvents.instance.CheckCosts();

        //loops through inventory, adds barrel from inputs to the first slot available, the returns out
        foreach (WorkshopBarrelSlot slot in inactiveInventoryBarrelSlots)
        {
            if (slot.slotType == BarrelType.Empty)
            {
                slot.slotType = (BarrelType)barrelNumber;
                slot.slotTier = (BarrelTeir)0;

                slot.UpdateDisplay();

                UIEvents.instance.CheckBarrelInventorySpace();
                return;
            }

        }
    }

    public void RepairBarricade()
    {
        if(Barricade.instance != null)
        {
            if(PriceCheckAndCharge(Barricade.instance.CaculateRepairCost()))
            {
                Barricade.instance.barricadeCurrentHealth += 11;
                Barricade.instance.BarricadeTakeDamage();
            }

        }
    }

    private bool PriceCheckAndCharge(int price)
    {
        if (scrap >= price)
        {
            scrap -= price;
            UIEvents.instance.UpdateScrapCounts();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CreateAddon()
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
        UIEvents.instance.CheckCosts();
        UIEvents.instance.CheckBarrelInventorySpace();
    }

    //adds to the first slot of the inactive inventory array (slots that are for storage and aren't on any gunheads)
    public void AddBarrelToInventory(int barrelNumber, int barrelTier)
    {
        //prevents tiers that dont exsist being added
        barrelTier = Mathf.Clamp(barrelTier, 0, 2);
        barrelNumber = Mathf.Clamp(barrelNumber, 0, 5);

        //loops through inventory, adds barrel from inputs to the first slot available, the returns out
        foreach (WorkshopBarrelSlot slot in inactiveInventoryBarrelSlots)
        {
            if (slot.slotType == BarrelType.Empty)
            {
                slot.slotType = (BarrelType)barrelNumber;
                slot.slotTier = (BarrelTeir)barrelTier;

                slot.UpdateDisplay();

                return;
            }
        }
    }

    public bool CheckInventoryFull()
    {
        foreach (WorkshopBarrelSlot slot in inactiveInventoryBarrelSlots)
        {
            if (slot.slotType == BarrelType.Empty)
            {
                return false;
            }

        }

        return true;
    }


    //v functions for ammo icon system <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    public bool CheckCanShoot(int arraySlot, int ammodrain)
    {
        if (clipQuantities[arraySlot] >= ammodrain)
        {
            return true;
        }
        return false;
    }

    public void InitilizeAllAmmoArrays()
    {

        //initilizes clip icons amount
        clipIconAmount = InitilizeAmmmoIntegerArray(clipIconAmount, pistolClipIcons, shotGunClipIcons, machineGunClipIcons, rockerLauncherClipIcons);
        //initilized ammo icons amount
        ammoIconAmount = InitilizeAmmmoIntegerArray(ammoIconAmount, pistolAmmoIcons, shotGunAmmoIcons, machineGunAmmoIcons, rockerLauncherAmmoIcons);
        //initilises Icon Values
        iconValues = InitilizeAmmmoIntegerArray(iconValues, pistolIconQuantity, shotGunIconQuantity, machineGunIconQuantity, rockerLauncherIconQuantity);
        //initilizes ammo and clip caps from icon amount and values
        ammoMaxes = InitilizeAmmmoIntegerArray(ammoMaxes, 0, 0, 0, 0);
        clipMaxes = InitilizeAmmmoIntegerArray(clipMaxes, 0, 0, 0, 0);
        SetAmmoCaps();
        //initilizes, then sets ammo and clip current to full
        ammoRegenCalcs = InitilizeAmmmoFloatArray(ammoRegenCalcs, 0, 0, 0, 0);
        ammoQuantities = InitilizeAmmmoIntegerArray(ammoQuantities, 0, 0, 0, 0);
        clipQuantities = InitilizeAmmmoIntegerArray(clipQuantities, 0, 0, 0, 0);
        FillAmmos();
        //initilizes regen rates
        ammoRegenRates = InitilizeAmmmoFloatArray(ammoRegenRates, pistolAmmoRegen, shotGunAmmoRegen, machineGunAmmoRegen, rocketLauncherAmmoRegen);

    }

    public bool AddClipIcons(int cost)
    {
        if (PriceCheckAndCharge(cost))
        {
            //increases icon quantities by values inputed
            clipIconAmount[0] += pistolClipIncrease;
            clipIconAmount[1] += shotGunClipIncrease;
            clipIconAmount[2] += machineGunClipIncrease;
            clipIconAmount[3] += rocketLauncherClipIncrease;

            //updates ammo caps and refills ammo
            SetAmmoCaps();
            FillAmmos();
            return true;
        }
        return false;
    }
    public bool AddAmmoIcons(int cost)
    {
        if (PriceCheckAndCharge(cost))
        {
            //increases icon quantities by values inputed
            ammoIconAmount[0] += pistolAmmoIncrease;
            ammoIconAmount[1] += shotGunAmmoIncrease;
            ammoIconAmount[2] += machineGunAmmoIncrease;
            ammoIconAmount[3] += rocketLauncherAmmoIncrease;

            //updates ammo caps and refills ammo
            SetAmmoCaps();
            FillAmmos();
            return true;
        }
        return false;
    }

    public bool IncreaseDamageMult(int cost)
    {
        if(PriceCheckAndCharge(cost))
        {
            //increases mult by value 
            damageMult += damageMultIncrease;

            return true;
        }
        return false;
    }

    public bool IncreaseFireSpeedMult(int cost)
    {
        if (PriceCheckAndCharge(cost))
        {
            //increases mult by value 
            fireSpeedMult += fireSpeedMultIncrease;

            return true;
        }
        return false;
    }

    public int[] InitilizeAmmmoIntegerArray(int[] array, int pistol, int shotGun, int machineGun, int rocketLauncher)
    {
        array = new int[4];
        array[0] = pistol;
        array[1] = shotGun;
        array[2] = machineGun;
        array[3] = rocketLauncher;

        return array;
    }
    public int[] UpdateAmmmoIntegerArray(int[] array, int pistol, int shotGun, int machineGun, int rocketLauncher)
    {
        //will cause an error if initilize hasent been ran due to no null/length check
        array[0] = pistol;
        array[1] = shotGun;
        array[2] = machineGun;
        array[3] = rocketLauncher;

        return array;
    }

    public float[] InitilizeAmmmoFloatArray(float[] array, float pistol, float shotGun, float machineGun, float rocketLauncher)
    {
        array = new float[4];
        array[0] = pistol;
        array[1] = shotGun;
        array[2] = machineGun;
        array[3] = rocketLauncher;

        return array;
    }
    public float[] UpdateAmmmoFloatArray(float[] array, float pistol, float shotGun, float machineGun, float rocketLauncher)
    {
        //will cause an error if initilize hasent been ran due to no null/length check
        array[0] = pistol;
        array[1] = shotGun;
        array[2] = machineGun;
        array[3] = rocketLauncher;

        return array;
    }

    public void SetAmmoCaps()
    {
        for (int i = 0; i < ammoMaxes.Length; i++)
        {
            ammoMaxes[i] = ammoIconAmount[i] * iconValues[i];
            clipMaxes[i] = clipIconAmount[i] * iconValues[i];
        }
    }

    public void FillAmmos()
    {
        for (int i = 0; i < ammoQuantities.Length; i++)
        {
            ammoRegenCalcs[i] = ammoMaxes[i];
            clipQuantities[i] = clipMaxes[i];
        }
    }

    public void ButtonSound()
    {
        InventoryScreenSFX.PlayInventorySFXClip(InventoryScreenSFX.purchaseSound);
    }
}
