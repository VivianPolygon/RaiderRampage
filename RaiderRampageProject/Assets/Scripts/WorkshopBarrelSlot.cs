using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopBarrelSlot : MonoBehaviour
{

    public bool slotEmpty;

    public BarrelType slotType;
    public BarrelTeir slotTier;

    private Vector2 slotData;

    public static WorkshopBarrelSlot slotScript;
    public static Vector2 dataStorage;
    public static Vector2 movingData;

    private GameObject slotIconObject;
    [HideInInspector]
    public Image slotImage;
    [HideInInspector]
    public Image iconImage;

    private Coroutine pickupSlot;

    private void Awake()
    {

        slotIconObject = transform.GetChild(0).gameObject;
        slotEmpty = false;

        if (slotIconObject.TryGetComponent<Image>(out Image Iconimage) && TryGetComponent<Image>(out Image image))
        {
            iconImage = Iconimage;
            slotImage = image;

        }
        else
        {
            Debug.LogWarning("Cant find the Image component on a Workshop Barrel Slot");
        }
    }

    private void OnEnable()
    {
        slotImage.enabled = true;
    }
    private void OnDisable()
    {
        iconImage.enabled = false;
        slotImage.enabled = false;
    }

    private void Start()
    {
        if (slotImage != null)
        {
            UpdateDisplay();
        }
    }

    public void SlotSendToData()
    {
        slotData.x = (int)slotType;
        slotData.y = (int)slotTier;
    }

    public void SlotReceiveFromData()
    {
        slotType = (BarrelType)slotData.x;
        slotTier = (BarrelTeir)slotData.y;
    }

    public void PickupSlot(bool pickedUp)
    {
        SlotSendToData();
        movingData = slotData;
        slotScript = this;

        pickupSlot = StartCoroutine(ContentsFollowTap(pickedUp));
    }
    public void DropSlot()
    {
        if (pickupSlot != null)
        {
            StopCoroutine(pickupSlot);
        }
        iconImage.transform.SetParent(slotImage.transform, true);
        iconImage.transform.position = slotImage.transform.position;

        if(slotType == BarrelType.Empty)
        {
            slotImage.enabled = true;
        }
        else
        {
            slotImage.enabled = false;
        }

    }

    public void SwapSlots()
    {
        //used for merging
        if (slotTier == slotScript.slotTier && slotType == slotScript.slotType && (int)slotTier < 2)
        {
            slotScript.slotType = BarrelType.Empty;
            slotScript.slotTier = BarrelTeir.Untiered;

            slotTier++;
        }
        //swaps slots of differing barrels
        else
        {
            SlotSendToData();
            dataStorage = slotData;
            slotData = movingData;
            SlotReceiveFromData();
            slotScript.slotData = dataStorage;
            slotScript.SlotReceiveFromData();
        }


        DropSlot();
        slotScript.DropSlot();

        UpdateDisplay();
        slotScript.UpdateDisplay();
    }

    private IEnumerator ContentsFollowTap(bool pickedUp)
    {
        while (pickedUp)
        {
            MergeingInputDetection.tapRay = Camera.main.ScreenPointToRay(MergeingInputDetection.tapInput.TapActionMap.TapPosition.ReadValue<Vector2>());
            iconImage.transform.SetParent(MergeingInputDetection.selectionObject.transform, true);
            slotImage.enabled = true;

            Physics.Raycast(MergeingInputDetection.tapRay, out MergeingInputDetection.tapHit);
            {
                iconImage.transform.position = MergeingInputDetection.tapHit.point;
            }

            yield return null;
        }
    }

    public void UpdateDisplay()
    {
        if (slotImage != null && iconImage != null)
        {
            switch (slotTier)
            {
                case BarrelTeir.Teir1:
                    iconImage.sprite = UIData.instance.gunTierShapes[0];
                    break;
                case BarrelTeir.Tier2:
                    iconImage.sprite = UIData.instance.gunTierShapes[1];
                    break;
                case BarrelTeir.Tier3:
                    iconImage.sprite = UIData.instance.gunTierShapes[2];
                    break;
                case BarrelTeir.Untiered:
                    iconImage.sprite = UIData.instance.gunTierShapes[3];
                    break;
                default:
                    break;
            }

            switch (slotType)
            {
                case BarrelType.SMG:
                    iconImage.color = UIData.instance.gunTypeColors[0];
                    slotImage.enabled = false;
                    break;
                case BarrelType.Pistol:
                    iconImage.color = UIData.instance.gunTypeColors[1];
                    slotImage.enabled = false;
                    break;
                case BarrelType.Shotgun:
                    iconImage.color = UIData.instance.gunTypeColors[2];
                    slotImage.enabled = false;
                    break;
                case BarrelType.MachineGun:
                    iconImage.color = UIData.instance.gunTypeColors[3];
                    slotImage.enabled = false;
                    break;
                case BarrelType.Sniper:
                    iconImage.color = UIData.instance.gunTypeColors[4];
                    slotImage.enabled = false;
                    break;
                case BarrelType.RocketLauncher:
                    iconImage.color = UIData.instance.gunTypeColors[5];
                    slotImage.enabled = false;
                    break;
                case BarrelType.Empty:
                    iconImage.color = UIData.instance.gunTypeColors[6];
                    iconImage.sprite = UIData.instance.gunTierShapes[3];
                    slotImage.enabled = true;
                    break;
                default:
                    break;
            }


        }
    }

}
