using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MergeingInputDetection : MonoBehaviour
{
    public static TapInput tapInput;

    public static Ray tapRay;
    public static RaycastHit tapHit;

    private bool barrelPickedUp;
    private WorkshopBarrelSlot currentBarrelSlot;

    [SerializeField]
    private GameObject selectionParentObject;
    public static GameObject selectionObject;

    private void Awake()
    {
        tapInput = new TapInput();
        barrelPickedUp = false;
        selectionObject = selectionParentObject;
    }

    private void OnEnable()
    {
        tapInput.Enable();
    }
    private void OnDisable()
    {
        tapInput.Disable();
    }

    private void Start()
    {
        tapInput.TapActionMap.TapInput.started += context => TapPressed(context);
        tapInput.TapActionMap.TapInput.canceled += context => TapReleased(context);
    }

    private void TapPressed(InputAction.CallbackContext context)
    {
        tapRay = Camera.main.ScreenPointToRay(tapInput.TapActionMap.TapPosition.ReadValue<Vector2>());

        Physics.Raycast(tapRay, out tapHit);
        {
            if (tapHit.collider.TryGetComponent<WorkshopBarrelSlot>(out WorkshopBarrelSlot barrelSlot) && !barrelSlot.slotImage.enabled)
            {
                barrelPickedUp = true;
                currentBarrelSlot = barrelSlot;
                currentBarrelSlot.PickupSlot(barrelPickedUp);
            }
        }
    }

    private void TapReleased(InputAction.CallbackContext context)
    {
        //print("ScreenReleased");

        tapRay = Camera.main.ScreenPointToRay(tapInput.TapActionMap.TapPosition.ReadValue<Vector2>());

        Physics.Raycast(tapRay, out tapHit);
        {
            if(barrelPickedUp)
            {
                //controls merging/swapping if another slot is selected
                if (tapHit.collider.TryGetComponent<WorkshopBarrelSlot>(out WorkshopBarrelSlot barrelSlot))
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
                if(tapHit.collider.tag == "ScrapBin")
                {
                    WorkshopBarrelSlot.slotScript.DropSlot();
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

        }

        barrelPickedUp = false;
    }

}
