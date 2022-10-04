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
    public Text[] ScrapQuantityTexts;
    public Text[] AddonQuantityTexts;

    //needed to make them disappear when they are irrelevent
    [Header("Upgrade Buttons")]
    [SerializeField]
    public Button slotAddonButton;

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

        //event subscriptions
        UIEvents.instance.onUpdateScrapCounts += UpdateScrapQuantityTexts;
        UIEvents.instance.onUpdateAddonCounts += UpdateAddonTexts;
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
        waveBarSlider.value = waveProgressPercent;
    }

    public void SetWaveText(int waveNumber)
    {
        waveText.text = ("Current Wave: " + waveNumber.ToString());
    }

    public void UpdateScrapQuantityTexts()
    {
        for (int i = 0; i < ScrapQuantityTexts.Length; i++)
        {
            ScrapQuantityTexts[i].text = ("Scrap: <size=40><color=white><b>" + PlayerResourcesManager.scrap.ToString() +  "</b></color></size>");
        }
    }
    public void UpdateAddonTexts()
    {
        for (int i = 0; i < AddonQuantityTexts.Length; i++)
        {
            AddonQuantityTexts[i].text = SlotAddonInventory.slotAddonQuantity.ToString();
        }
    }
}
