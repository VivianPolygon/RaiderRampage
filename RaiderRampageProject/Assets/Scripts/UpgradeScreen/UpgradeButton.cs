using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public enum UpgradeType
    {
        damage,
        fireSpeed,
        ammoCapacity,
        clipCapacity
    }

    public UpgradeType upgradeType;
    public string buttonLabelText;

    public int baseCost = 10; // base price
    public float costMultiplier = 1.5f; //functions exponentialy
    public int upgradesMax = 3; // max amount of upgrades

    private int currentCost;
    private int upgradeCountCurrent;

    private Text buttonText;


    // Start is called before the first frame update
    void Start()
    {
        currentCost = baseCost;
        upgradeCountCurrent = 0;

        if(transform.childCount > 0)
        {
            if(transform.GetChild(0).TryGetComponent(out Text text))
            {
                buttonText = text;
            }
            else
            {
                Debug.LogWarning("Button's first child has no text component: " + gameObject.name + ", make sure the first child has a Text component, Button Gameobject deleted at runtime");
                Destroy(this.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Button has no children, make sure the button: " + gameObject.name + "has a child and this child has a Text Component, Button Gameobject deleted at runtime");
            Destroy(this.gameObject);
        }

        SetButtonText();
    }

    public void Upgrade()
    {
        if (upgradeCountCurrent < upgradesMax)
        {
            switch (upgradeType)
            {
                case UpgradeType.damage:
                    if (PlayerResourcesManager.instance.IncreaseDamageMult(currentCost))
                    {
                        //updates internal tracking on button for amount of upgrades
                        upgradeCountCurrent++;
                        //caculates cost for next upgrade
                        currentCost = Mathf.RoundToInt(currentCost * costMultiplier);

                        //Updates the mult count at the top of the upgrade screen
                        UIEvents.instance.UpdateMults();
                    }
                    break;
                case UpgradeType.fireSpeed:
                    if(PlayerResourcesManager.instance.IncreaseFireSpeedMult(currentCost))
                    {
                        //updates internal tracking on button for amount of upgrades
                        upgradeCountCurrent++;
                        //caculates cost for next upgrade
                        currentCost = Mathf.RoundToInt(currentCost * costMultiplier);

                        //Updates the mult count at the top of the upgrade screen
                        UIEvents.instance.UpdateMults();
                    }
                    break;
                case UpgradeType.ammoCapacity:
                    if (PlayerResourcesManager.instance.AddAmmoIcons(currentCost))
                    {
                        //updates internal tracking on button for amount of upgrades
                        upgradeCountCurrent++;
                        //caculates cost for next upgrade
                        currentCost = Mathf.RoundToInt(currentCost * costMultiplier);

                        //sets the game state managers tracking for if an ammo upgrade was purchased to true, updates icons when wave restarts
                        GameStateManager.instance.SetAmmoUpgradeTrue();
                        //updates counts at top of upgrade screen displaying the number of icons
                        UIEvents.instance.UpdateAmmoIconCount();
                    }
                    break;
                case UpgradeType.clipCapacity:
                    if (PlayerResourcesManager.instance.AddClipIcons(currentCost))
                    {
                        //updates internal tracking on button for amount of upgrades
                        upgradeCountCurrent++;
                        //caculates cost for next upgrade
                        currentCost = Mathf.RoundToInt(currentCost * costMultiplier);

                        //sets the game state managers tracking for if an ammo upgrade was purchased to true, updates icons when wave restarts
                        GameStateManager.instance.SetAmmoUpgradeTrue();
                        //updates counts at top of upgrade screen displaying the number of icons
                        UIEvents.instance.UpdateAmmoIconCount();
                    }
                    break;
                default:
                    break;
            }
            SetButtonText();
        }

        if(upgradeCountCurrent >= upgradesMax)
        {
            Destroy(this.gameObject);
        }
    }

    //sets button text using current cost and inputed button label
    private void SetButtonText()
    {
        buttonText.text = (buttonLabelText + "<b><size=17> " + currentCost.ToString() + " Scrap </size></b>");
    }

}
