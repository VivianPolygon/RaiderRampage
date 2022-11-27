using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Script used on the barrel slots in the workshop/merging screen, each slot behaves with any other slot in the same way irregardless of where it is,
//as long as its not locked from WorkshopExtraSlotLock

public class WorkshopBarrelSlot : MonoBehaviour
{
    //barrel data
    public BarrelType slotType;
    public BarrelTeir slotTier;
    //combined ray numbers from the previous two enums
    private Vector2 slotData;
    //single shared instance of a slotscripts, used to save an instance of a script when swapping data between slots
    public static WorkshopBarrelSlot slotScript;
    //stores a slotData vector 2, for data swapping
    public static Vector2 dataStorage;
    //stores a slotData vector 2, for data swapping
    public static Vector2 movingData;

    //gameobject for this barrelslot's image
    private GameObject slotIconObject;
    //slot background image
    [HideInInspector]
    public Image slotImage;
    //image used for the contents, changes depending on the data contained
    [HideInInspector]
    public Image iconImage;
    //coroutine used for draggin icons around
    private Coroutine pickupSlot;

    private void Awake()
    {
        //gets the child (there should only be one) and logs a warning if that child dosent have an image component
        slotIconObject = transform.GetChild(0).gameObject;

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

    //enables the images when the script is enabled, needed for WorkshopExtraSlotLock to work properly and easily
    private void OnEnable()
    {
        iconImage.enabled = true;
        slotImage.enabled = true;
    }
    //disables the images when the script is disabled, needed for WorkshopExtraSlotLock to work properly and easily
    private void OnDisable()
    {
        iconImage.enabled = false;
        slotImage.enabled = false;
    }

    //initilizes the display, as long as the images are present
    private void Start()
    {
        if (slotImage != null)
        {
            UpdateDisplay();
        }
    }

    //sends the enums to the vector 2 for transfer
    public void SlotSendToData()
    {
        slotData.x = (int)slotType;
        slotData.y = (int)slotTier;
    }

    //sends the vector 2 back to the enums to be used by the slot
    public void SlotReceiveFromData()
    {
        slotType = (BarrelType)slotData.x;
        slotTier = (BarrelTeir)slotData.y;
    }

    //picks up the slots data, uses the vector 2
    public void PickupSlot(bool pickedUp)
    {
        SlotSendToData();
        movingData = slotData;
        slotScript = this;

        pickupSlot = StartCoroutine(ContentsFollowTap(pickedUp));
    }
    //drops the slot back to its initial position, used if the slot isint dragged over a new vailid slot where it would either swap or merge
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
    //either swaps or merges the slots
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

    //drags the icon around
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
    //updates the icon for the slot depending on the value of the enums, changes the color depending on type, and the shape depending on tier
    //color and icon data is held in UI data on the game manager
    public void UpdateDisplay()
    {
        if (slotImage != null && iconImage != null)
        {

            switch (slotType)
            {
                case BarrelType.SMG:
                    iconImage.enabled = true;
                    iconImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.SMGSpriteNumber + (int)slotTier);
                    slotImage.enabled = false;
                    break;
                case BarrelType.Pistol:
                    iconImage.enabled = true;
                    iconImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.pistolSpriteNumber + (int)slotTier);
                    slotImage.enabled = false;
                    break;
                case BarrelType.Shotgun:
                    iconImage.enabled = true;
                    iconImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.shotgunSpriteNumber + (int)slotTier);
                    slotImage.enabled = false;
                    break;
                case BarrelType.MachineGun:
                    iconImage.enabled = true;
                    iconImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.machinegunSpriteNumber + (int)slotTier);
                    slotImage.enabled = false;
                    break;
                case BarrelType.Sniper:
                    iconImage.enabled = true;
                    iconImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.sniperSpriteNumber + (int)slotTier);
                    slotImage.enabled = false;
                    break;
                case BarrelType.RocketLauncher:
                    iconImage.enabled = true;
                    iconImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.rocketLauncherSpriteNumber + (int)slotTier);
                    slotImage.enabled = false;
                    break;
                case BarrelType.Empty:
                    slotImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.emptySlot);
                    slotImage.enabled = true;
                    iconImage.enabled = false;
                    break;
                default:
                    break;
            }


        }
    }

}
