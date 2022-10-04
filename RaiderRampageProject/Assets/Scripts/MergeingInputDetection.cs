using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//script used for tracking iputs during the merge phase of the game, as well as applying logic to those inputs from functions using raycasts
public class MergeingInputDetection : MonoBehaviour
{
    //instance of the actionmap script, detects the first touch on the screen only, tracks the tap down and release
    public static TapInput tapInput;
    //raycast variables
    public static Ray tapRay;
    public static RaycastHit tapHit;

    //flag for detecting if a barrel has been picked up for merging/swaping
    private bool barrelPickedUp;
    //current barrel slot picked up from
    private WorkshopBarrelSlot currentBarrelSlot;

    //flag used for detecting if a addon has been picked up
    private bool slotAddonPickedUp;
    //current SlotAddonInventory Script (theres only 1 in the scene) <<< THERE WILL BE ISSUES IF A ADDON STARTS EQUIPT, HOWEVER THIS SHOULDNT BE THE CASE
    private SlotAddonInventory addonInventory;

    //onject used for selection, rendered at the highest layer of the canvas, used to make dragged sprites appear above other sprites
    [SerializeField]
    private GameObject selectionParentObject;
    public static GameObject selectionObject;

    private void Awake()
    {
        //initilizes
        tapInput = new TapInput();
        barrelPickedUp = false;
        slotAddonPickedUp = false;
        selectionObject = selectionParentObject;
    }

    private void OnEnable()
    {
        //enables the tapinput script
        tapInput.Enable();
    }
    private void OnDisable()
    {
        //disables the tap script incase this is ever disabled
        tapInput.Disable();
    }

    private void Start()
    {
        //subscribes to the start and cancel events of the first touch on the screen
        tapInput.TapActionMap.TapInput.started += context => TapPressed(context);
        tapInput.TapActionMap.TapInput.canceled += context => TapReleased(context);
    }

    //raycast used fro when the tap is pressed down
    private void TapPressed(InputAction.CallbackContext context)
    {
        //sets the ray to where the tap has occured in world space, using the main camera and input in screen space
        tapRay = Camera.main.ScreenPointToRay(tapInput.TapActionMap.TapPosition.ReadValue<Vector2>());

        //fires the raycast
        Physics.Raycast(tapRay, out tapHit);
        {
            //if the collider hit is a workshop barrel slot, and the script is enabled and the slot image is enabled, (meaning it has contents) picks up that contents
            //sets flag for barrel picked up
            if (tapHit.collider.TryGetComponent(out WorkshopBarrelSlot barrelSlot) && !barrelSlot.slotImage.enabled && barrelSlot.enabled)
            {
                barrelPickedUp = true;
                currentBarrelSlot = barrelSlot;
                currentBarrelSlot.PickupSlot(barrelPickedUp);
            }
            //if the collider his is the slot addon stock, does similar to above, but for slot addons
            //sets flag for addon being picked up
            if (tapHit.collider.TryGetComponent(out SlotAddonInventory slotAddonInventory))
            {
                addonInventory = slotAddonInventory;

                if (slotAddonInventory.PickupAddon())
                {
                    slotAddonPickedUp = true;
                }
            }
            //if a workshop barrel slot has a lock on it, removes the addon, and picks it up (if it has one)
            //sets flag for addon being picked up
            if (addonInventory != null && tapHit.collider.TryGetComponent(out WorkshopExtraSlotLock slotLock) && tapHit.collider.TryGetComponent(out WorkshopBarrelSlot slotBarrel))
            {
                if(addonInventory.RemoveAddon(slotLock, slotBarrel))
                {
                    slotAddonPickedUp = true;
                }
            }
        }
    }

    //raycast used for when the tap is released
    private void TapReleased(InputAction.CallbackContext context)
    {

        //sets the ray to where the tap has occured in world space, using the main camera and input in screen space
        tapRay = Camera.main.ScreenPointToRay(tapInput.TapActionMap.TapPosition.ReadValue<Vector2>());

        //fires the raycast
        Physics.Raycast(tapRay, out tapHit);
        {
            //if the flag for having a barrel is set from when a tap was pressed down, this runs
            if(barrelPickedUp)
            {
                //controls merging/swapping if another slot is selected
                if (tapHit.collider.TryGetComponent(out WorkshopBarrelSlot barrelSlot) && barrelSlot.enabled)
                {
                    if(barrelSlot == WorkshopBarrelSlot.slotScript)
                    {
                        currentBarrelSlot.DropSlot();
                    }
                    else
                    {
                        barrelSlot.SwapSlots();
                    }
                }
                //controls scrapping mechanic if hovered over an object woth a collider and thats tagged as "ScrapBin"
                if (tapHit.collider.TryGetComponent(out ScrapItem scrapItem))
                {
                    WorkshopBarrelSlot.slotScript.DropSlot();
                    scrapItem.ScrapBarrel(WorkshopBarrelSlot.slotScript.slotType, WorkshopBarrelSlot.slotScript.slotTier);

                    WorkshopBarrelSlot.slotScript.slotType = BarrelType.Empty;
                    WorkshopBarrelSlot.slotScript.slotTier = BarrelTeir.Untiered;
                    WorkshopBarrelSlot.slotScript.UpdateDisplay();

                }
                //snaps the slot back if nothing is selected
                else
                {
                    currentBarrelSlot.DropSlot();
                }
            }
            //if the addon slot flag was set, this runs
            if(slotAddonPickedUp && addonInventory != null)
            {
                if(tapHit.collider.TryGetComponent(out WorkshopExtraSlotLock lockSlot))
                {
                    addonInventory.PlaceAddon(lockSlot);
                }
                else
                {
                    addonInventory.DropAddon();
                }

            }

        }

        //sets the flags back to false for when the logic runs next
        barrelPickedUp = false;
        slotAddonPickedUp = false;
    }

}
