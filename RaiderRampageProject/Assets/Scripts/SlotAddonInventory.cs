using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Script used to control the stock of slot addons, and house their functions for MergineInputDetection to use in it's logic
public class SlotAddonInventory : MonoBehaviour
{
    public static SlotAddonInventory instance;

    //static total count of addons in storage
    public static int slotAddonQuantity;

    //initial quantity of addons, sets slotAddonQuantity in start, then does nothing
    private int startingAddonQuantity;
    //maximum amout of addonslots, should be set to the maximum available (12 as of writing), will always total out to whats set, between stock and equipt
    //IE, if 5 are equipt and there is a total of 12, stock wont be able to excede 7

    //current amount equipt, used to create above behavior
    public static int currentAddonEquipt;

    //same as total, but set at start from total, used to prevent changes at runtime throwing the count off
    private int addonQuantityMax;

    //GameObject thats made active when dragging a slot, follows tap using a raycast
    [SerializeField]
    private Transform dragIcon;

    //coroutine used for dragging
    private Coroutine pickupCoroutine;

    //colors used to tint the sprite depending if there is any in stock or not
    [SerializeField]
    private Color addonSlotsPresentColor;
    [SerializeField]
    private Color noAddonSlotsPresentColor;
    //image thats tinted, main image of the stock
    [SerializeField]
    private Image addonInventoryImage;

    void Awake()
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

    void Start()
    {
        DropAddon();

        startingAddonQuantity = PlayerResourcesManager.instance.startingSlotAddonQuantity;

        currentAddonEquipt = 0;
        addonQuantityMax = PlayerResourcesManager.instance.addonCap; ;
        slotAddonQuantity = Mathf.Clamp(startingAddonQuantity, 0, addonQuantityMax);

        UpdateCountText();
        SetAddonSprites();
    }

    //updates the text for the count, and updates color of the image
    private void UpdateCountText()
    {
        UIEvents.instance.UpdateAddonCounts();
        SetColorFromCount(slotAddonQuantity);
    }

    //runs UpdateCountText and clamps the ammount that can be in stock
    public void AddSlotAddons(int amount)
    {
        slotAddonQuantity = Mathf.Clamp(slotAddonQuantity + amount, 0, addonQuantityMax - currentAddonEquipt);
        UpdateCountText();
    }

    //picks up from inventory
    public bool PickupAddon()
    {
        if(slotAddonQuantity > 0)
        {
            if (pickupCoroutine != null)
            {
                StopCoroutine(pickupCoroutine);
            }

            dragIcon.gameObject.SetActive(true);
            AddSlotAddons(-1);
            pickupCoroutine = StartCoroutine(AddonDrag());

            return true;
        }
        return false;
    }

    //drops back to inventory
    public void DropAddon()
    {
        if(pickupCoroutine != null)
        {
            StopCoroutine(pickupCoroutine);
        }

        AddSlotAddons(1);
        dragIcon.gameObject.SetActive(false);
                SetColorFromCount(slotAddonQuantity);
    }

    //places addon on a gunhead
    public void PlaceAddon(WorkshopExtraSlotLock lockSlot)
    {
        if(lockSlot.locked)
        {
            if (pickupCoroutine != null)
            {
                StopCoroutine(pickupCoroutine);
            }
            currentAddonEquipt++;

            dragIcon.gameObject.SetActive(false);
            lockSlot.Locked(false);

        }
        else
        {
            DropAddon();
        }

    }
    //removes from gunhead and picks up
    public bool RemoveAddon(WorkshopExtraSlotLock lockSlot, WorkshopBarrelSlot barrelSlot)
    {
        if (barrelSlot.slotType == BarrelType.Empty && !lockSlot.locked)
        {
            if (pickupCoroutine != null)
            {
                StopCoroutine(pickupCoroutine);
            }
            currentAddonEquipt--;

            dragIcon.gameObject.SetActive(true);
            pickupCoroutine = StartCoroutine(AddonDrag());

            lockSlot.Locked(true);

            return true;
        }
        return false;
    }

    //coroutine used for dragging the icon
    private IEnumerator AddonDrag()
    {
        while (true)
        {
            //uses the raycast from merging input detection, caculated the same way from how it is there
            MergeingInputDetection.tapRay = Camera.main.ScreenPointToRay(MergeingInputDetection.tapInput.TapActionMap.TapPosition.ReadValue<Vector2>());

            //fires ray and moves drag object to its collision point
            Physics.Raycast(MergeingInputDetection.tapRay, out MergeingInputDetection.tapHit);
            {
                dragIcon.position = MergeingInputDetection.tapHit.point;
            }

            yield return null;
        }
    }

    //sets the color depending on the count
    private void SetColorFromCount(int inventoryAddonCount)
    {
        if(inventoryAddonCount > 0)
        {
            addonInventoryImage.color = addonSlotsPresentColor;
        }
        else
        {
            addonInventoryImage.color = noAddonSlotsPresentColor;
        }
    }

    private void SetAddonSprites()
    {
        addonInventoryImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.slotAddonIconSpriteNumber);

        if(dragIcon.TryGetComponent(out Image iconImage))
        {
            iconImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.slotAddonIconSpriteNumber);
        }
        else
        {
            Debug.LogWarning("The Object used to display the dragging of an addonsot does not have an image component, please add one");
        }
    }
}
