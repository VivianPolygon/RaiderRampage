using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoIconsQuantityDisplay : MonoBehaviour
{
    private Text[] displayTexts;
    [SerializeField] private DisplayType displayType;

    public enum DisplayType
    {
        ammoCounts,
        clipCounts
    }

    private void Awake()
    {
        FindChildrenWithText();
    }

    private void OnEnable()
    {
        UIEvents.instance.onUpdateAmmoIconCount += UpdateTexts;
    }
    private void OnDisable()
    {
        UIEvents.instance.onUpdateAmmoIconCount -= UpdateTexts;
    }

    private void FindChildrenWithText()
    {
        displayTexts = new Text[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Text text))
            {
                displayTexts[i] = text;
            }
            else
            {
                Debug.Log("A child on the display for ammo/clip icons on the upgrade screen does not have a text component, please check that all children of: " + gameObject.name + "have a text compontent. object deleted on runtime");
                Destroy(transform.GetChild(i));
            }
        }
    }

    private void UpdateTexts()
    {
        switch (displayType)
        {
            case DisplayType.ammoCounts:
                for (int i = 0; i < PlayerResourcesManager.instance.ammoIconAmount.Length; i++)
                {
                    displayTexts[i].text = PlayerResourcesManager.instance.ammoIconAmount[i].ToString();
                }
                break;
            case DisplayType.clipCounts:
                for (int i = 0; i < PlayerResourcesManager.instance.clipIconAmount.Length; i++)
                {
                    displayTexts[i].text = PlayerResourcesManager.instance.clipIconAmount[i].ToString();
                }
                break;
            default:
                break;
        }
    }

}


