using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrelButtonsInventoryChecker : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private Text inventoryFullText;


    private void Awake()
    {
        inventoryFullText.enabled = false;
    }

    private void OnEnable()
    {
        UIEvents.instance.onCheckBarrelInventorySpace += BarrelButtonsCheckInventoryFull;
    }

    private void OnDisable()
    {
        UIEvents.instance.onCheckBarrelInventorySpace -= BarrelButtonsCheckInventoryFull;
    }

    public void BarrelButtonsCheckInventoryFull()
    {
        if (PlayerResourcesManager.instance.CheckInventoryFull())
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
                inventoryFullText.enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = true;
                inventoryFullText.enabled = false;
            }
            UIEvents.instance.CheckCosts();
        }
    }
}
