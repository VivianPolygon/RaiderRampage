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

    //sliders for the ammo bars and fills
    [SerializeField]
    private Slider[] ammoSliders;
    [SerializeField]
    private Image[] fillImages;

    public Slider ammoReloadTimerSlider;

    //colors for the bar fills
    [SerializeField] 
    private Color sliderEmptyColor;
    [SerializeField]
    private Color sliderFullColor;

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
        //initilizes the sliders
        SetAllAmmoSliders();
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
            scrapQuantityTexts[i].text = ("Scrap: <size=40><color=white><b>" + PlayerResourcesManager.scrap.ToString() +  "</b></color></size>");
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

    
}
