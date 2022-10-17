using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Script that holds UI elements and their data
public class UIData : MonoBehaviour
{
    //singleton instance
    public static UIData instance;

    public Canvas shootingControlsCanvas;
    public Canvas mergeingCanvas;
    public Canvas upgradesCanvas;

    public Slider ammoReloadTimerSlider;

    [Header("AmmoDrainIcons")]
    [SerializeField] private GameObject pistolAmmoDrainIcons;
    [SerializeField] private GameObject shotGunAmmoDrainIcons;
    [SerializeField] private GameObject machineGunAmmoDrainIcons;
    [SerializeField] private GameObject rocketLauncherAmmoDrainIcons;

    private GameObject[] ammoDrainIconParents;

    [Header("ClipDrainIcons")]
    [SerializeField] private GameObject pistolClipDrainIcons;
    [SerializeField] private GameObject shotGunClipDrainIcons;
    [SerializeField] private GameObject machineGunClipDrainIcons;
    [SerializeField] private GameObject rocketLauncherClipDrainIcons;

    private GameObject[] clipDrainIconParents;

    [Header("DrainIconPrefabs")]
    [SerializeField] private GameObject pistolDrainIconPrefab;
    [SerializeField] private GameObject shotGunDrainIconPrefab;
    [SerializeField] private GameObject machineGunDrainIconPrefab;
    [SerializeField] private GameObject rocketLauncherDrainIconPrefab;

    private GameObject[] drainIconPrefabs;

    //arrays of the ammo drain icon sliders
    private Slider[][] ammoDrainIconSliders;
    //arrays of the clip drain icon sliders
    private Slider[][] clipDrainIconSliders;

    [Header("SpriteSheets")]
    public Texture largeSpriteSheet;
    [HideInInspector]
    public Sprite[] largeSprites;

    [Header("ShootingUIImages")]
    public Image fireButtonImage;
    public Image reloadButtonImage;
    public Image aimingStickImage;

    public Image scrapBarrelArrowsImage;

    [Header("FireButton")]
    [Header("ShootingUISpriteSheetNumbers")]
    public short inactiveFireButtonSpriteNumber;
    public short activeFireButtonSpriteNumber;
    [Header("ReloadButton")]
    public short inactiveReloadButtonSpriteNumber;
    public short activeReloadButtonSpriteNumber;
    [Header("ThumbstickIcon")]
    public short aimingStickSpriteNumber;

    [Header("Make Tiers Consecutive on Sheet")]
    [Header("WorkshopBarrels, First Slot")]
    public short neutralReticle;
    public short SMGSpriteNumber;
    public short pistolSpriteNumber;
    public short machinegunSpriteNumber;
    public short shotgunSpriteNumber;
    public short sniperSpriteNumber;
    public short rocketLauncherSpriteNumber;

    [Header("WorkshopIcons")]
    public short emptySlot;
    public short scrapBarrelArrowsSpriteNumber;
    public short slotAddonIconSpriteNumber;

    [Header("WaveBar")]
    [SerializeField]
    private Slider waveBarSlider;
    [SerializeField]
    private Text waveText;

    [Header("UpgradeResourcesTexts")]
    public Text[] scrapQuantityTexts;
    public Text[] addonQuantityTexts;

    [Header("Indicators For Time Remaining for Merging/Upgrading")]
    public Image[] timerIndicators;

    [Header("Gunhead Swapping Images")]
    public Image[] gunheadActiveImages;
    public Color gunheadImageActiveColor;
    public Color gunheadImageInactiveColor;


    //needed to make them disappear when they are irrelevent
    [Header("Upgrade Buttons")]
    [SerializeField]
    public Button slotAddonButton;

    [Header("Objects Visible Only After a Wave On Shooting Canvas")]
    public GameObject[] waveEndObjects;

    [Header("Text of Previous Buttons")]
    public Text repairBarricadeText;




    //establishes the singleton, initilizes spritesheets from resources folder
    private void Awake()
    {
        largeSprites = Resources.LoadAll<Sprite>(largeSpriteSheet.name);

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
        InitilizeShootingUISprites();

        SetWaveBar(0);
        waveText.text = ("-------------->");

        scrapBarrelArrowsImage.sprite = SetSpriteFromLargeSheet(scrapBarrelArrowsSpriteNumber);

        //event subscriptions for UI events
        UIEvents.instance.onUpdateScrapCounts += UpdateScrapQuantityTexts;
        UIEvents.instance.onUpdateAddonCounts += UpdateAddonTexts;

        UIEvents.instance.onUpdateBarricade += UpdateRepairBarricadeTextAndButton;
        //event subscriptions for GameStateManger events
        GameStateManager.instance.onWaveEnd += ShowPostWaveUI;
        GameStateManager.instance.onWaveStart += HidePostWaveUI;


        ammoDrainIconSliders = new Slider[4][];
        clipDrainIconSliders = new Slider[4][];

        InitilizeDrainIcons();
        SetDrainIcons();

        UpdateAllDrainIcons();
    }

    private void OnDisable()
    {
        //event unsubscriptions for UI events
        UIEvents.instance.onUpdateScrapCounts -= UpdateScrapQuantityTexts;
        UIEvents.instance.onUpdateAddonCounts -= UpdateAddonTexts;

        UIEvents.instance.onUpdateBarricade -= UpdateRepairBarricadeTextAndButton;
        //event unsubscriptions for GameStateManger events
        GameStateManager.instance.onWaveEnd -= ShowPostWaveUI;
        GameStateManager.instance.onWaveStart -= HidePostWaveUI;
    }

    public Sprite SetSpriteFromLargeSheet(int spriteNumber)
    {
        spriteNumber = Mathf.Clamp(spriteNumber, 0, largeSprites.Length - 1);

        return largeSprites[spriteNumber];
    }

    private void InitilizeShootingUISprites()
    {
        fireButtonImage.sprite = SetSpriteFromLargeSheet(inactiveFireButtonSpriteNumber);
        reloadButtonImage.sprite = SetSpriteFromLargeSheet(inactiveReloadButtonSpriteNumber);
        aimingStickImage.sprite = SetSpriteFromLargeSheet(aimingStickSpriteNumber);
    }

    public void SetWaveBar(float waveProgressPercent)
    {
        if(waveBarSlider != null)
        {
            waveBarSlider.value = waveProgressPercent;
        }
    }

    public void SetWaveText(int waveNumber)
    {
        waveText.text = ("Current Wave: " + waveNumber.ToString());
    }

    public void UpdateScrapQuantityTexts()
    {
        for (int i = 0; i < scrapQuantityTexts.Length; i++)
        {
            if(scrapQuantityTexts[i] != null)
            {
                scrapQuantityTexts[i].text = ("Scrap: <size=40><color=white><b>" + PlayerResourcesManager.scrap.ToString() + "</b></color></size>");
            }
        }
    }
    public void UpdateAddonTexts()
    {
        for (int i = 0; i < addonQuantityTexts.Length; i++)
        {
            addonQuantityTexts[i].text = SlotAddonInventory.slotAddonQuantity.ToString();
        }
    }

    public void UpdateRepairBarricadeTextAndButton()
    {
        if(Barricade.instance.CaculateRepairScrap() > 0)
        {
            repairBarricadeText.text = ("Repair Baricade: " + Barricade.instance.CaculateRepairScrap().ToString() + " Scrap");
            repairBarricadeText.transform.parent.GetComponent<Button>().interactable = true;
            
        }
        else
        {
            repairBarricadeText.transform.parent.GetComponent<Button>().interactable = false;
            repairBarricadeText.text = ("Barricade Maxxed");
        }

    }

    public void HidePostWaveUI()
    {
        for (int i = 0; i < waveEndObjects.Length; i++)
        {
            if (waveEndObjects[i] != null)
            {
                waveEndObjects[i].SetActive(false);
            }
        }

        for (int i = 0; i < timerIndicators.Length; i++)
        {
            if(timerIndicators[i] != null)
            {
                timerIndicators[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowPostWaveUI()
    {
        for (int i = 0; i < waveEndObjects.Length; i++)
        {
            if(waveEndObjects[i] != null)
            {
                waveEndObjects[i].SetActive(true);
            }
        }

        for (int i = 0; i < timerIndicators.Length; i++)
        {
            if(timerIndicators[i] != null)
            {
                timerIndicators[i].gameObject.SetActive(true);
            }
        }
    }

    //Functions coresponding to the ammo drain icons AKA the zelda heart system for ammo

    public GameObject[] InitilizeGunsGameObjectArray(GameObject[] array, GameObject pistol, GameObject shotGun, GameObject machineGun, GameObject rocketLauncher)
    {
        array = new GameObject[4];
        array[0] = pistol;
        array[1] = shotGun;
        array[2] = machineGun;
        array[3] = rocketLauncher;

        return array;
    }

    public GameObject[] UpdateAmmmoIntegerArray(GameObject[] array, GameObject pistol, GameObject shotGun, GameObject machineGun, GameObject rocketLauncher)
    {
        array[0] = pistol;
        array[1] = shotGun;
        array[2] = machineGun;
        array[3] = rocketLauncher;

        return array;
    }

    public Slider[] InitilizeSliderArray(Slider[] array, int length)
    {
        array = new Slider[length];

        return array;
    }


    private void InitilizeDrainIcons()
    {
        drainIconPrefabs = InitilizeGunsGameObjectArray(drainIconPrefabs, pistolDrainIconPrefab, shotGunDrainIconPrefab, machineGunDrainIconPrefab, rocketLauncherDrainIconPrefab);
        ammoDrainIconParents = InitilizeGunsGameObjectArray(ammoDrainIconParents, pistolAmmoDrainIcons, shotGunAmmoDrainIcons, machineGunAmmoDrainIcons, rocketLauncherAmmoDrainIcons);
        clipDrainIconParents = InitilizeGunsGameObjectArray(clipDrainIconParents, pistolClipDrainIcons, shotGunClipDrainIcons, machineGunClipDrainIcons, rocketLauncherClipDrainIcons);
    }

    private void SetDrainIcons()
    {
        GameObject newIcon;

        for (int i = 0; i < drainIconPrefabs.Length; i++)
        {
            //destroys current Icons
            foreach(Transform child in ammoDrainIconParents[i].transform)
            {
                Destroy(child);
            }
            foreach (Transform child in clipDrainIconParents[i].transform)
            {
                Destroy(child);
            }

            //initilizes the length of the coresponding slider array
            ammoDrainIconSliders[i] = InitilizeSliderArray(ammoDrainIconSliders[i], PlayerResourcesManager.instance.ammoIconAmount[i]);
            //gets the ammount of AMMO icons from the coresponding ammo type from PlayerResourcesManager
            for (int ii = 0; ii < PlayerResourcesManager.instance.ammoIconAmount[i]; ii++)
            {
                newIcon = Instantiate(drainIconPrefabs[i]);
                newIcon.transform.SetParent(ammoDrainIconParents[i].transform);
                newIcon.transform.localScale = Vector3.one;

                //sets the corresponding sliders within the array on the array of arrays
                if(newIcon.TryGetComponent(out Slider slider))
                {
                    ammoDrainIconSliders[i][ii] = slider;
                }
                else
                {
                    Debug.LogWarning("An Ammo Drain Prefab Does not have a slider on it on the parent of it");
                }
                
            }

            //initilizes the length of the coresponding slider array
            clipDrainIconSliders[i] = InitilizeSliderArray(clipDrainIconSliders[i], PlayerResourcesManager.instance.clipIconAmount[i]);
            //gets the ammount of CLIP icons from the coresponding ammo type from PlayerResourcesManager
            for (int ii = 0; ii < PlayerResourcesManager.instance.clipIconAmount[i]; ii++)
            {
                newIcon = Instantiate(drainIconPrefabs[i]);
                newIcon.transform.SetParent(clipDrainIconParents[i].transform);
                newIcon.transform.localScale = Vector3.one;

                //sets the corresponding sliders within the array on the array of arrays
                if (newIcon.TryGetComponent(out Slider slider))
                {
                    clipDrainIconSliders[i][ii] = slider;
                }
                else
                {
                    Debug.LogWarning("An Ammo Drain Prefab Does not have a slider on it on the parent of it");
                }
            }
        }
    }

    public void UpdateAllDrainIcons()
    {
        UpdateClipDrainIcons();
        UpdateAmmoDrainIcons();
    }

    public void UpdateClipDrainIcons()
    {
        for (int i = 0; i < clipDrainIconSliders.Length; i++)
        {
            for (int ii = 0; ii < clipDrainIconSliders[i].Length; ii++)
            {
                clipDrainIconSliders[i][ii].value = ((float)PlayerResourcesManager.instance.clipQuantities[i] / PlayerResourcesManager.instance.iconValues[i]) - ii;
            }
        }
    }
    
    public void UpdateAmmoDrainIcons()
    {
        for (int i = 0; i < ammoDrainIconSliders.Length; i++)
        {
            for (int ii = 0; ii < ammoDrainIconSliders[i].Length; ii++)
            {
                ammoDrainIconSliders[i][ii].value = ((float)PlayerResourcesManager.instance.ammoQuantities[i] / PlayerResourcesManager.instance.iconValues[i]) - ii;
            }
        }
    }

    public void UpdateSpecificClipDrainIcon(int iconsIndex)
    {
        for (int ii = 0; ii < clipDrainIconSliders[iconsIndex].Length; ii++)
        {
            clipDrainIconSliders[iconsIndex][ii].value = ((float)PlayerResourcesManager.instance.clipQuantities[iconsIndex] / PlayerResourcesManager.instance.iconValues[iconsIndex]) - ii;
        }
    }
}
