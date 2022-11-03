using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateBarrelButton : MonoBehaviour
{
    [SerializeField]
    private BarrelType buttonBarrelType;
    [SerializeField]
    private string gunName;
    //assumes on object
    private Button button;
    //assumed on child
    private Text buttonText;
    private int cost;


    private void Awake()
    {
        if (TryGetComponent(out Button barrelButton))
        {
            button = barrelButton;
        }
        if (transform.GetChild(0).TryGetComponent(out Text text))
        {
            buttonText = text;
        }

        if (button == null || buttonText == null)
        {
            Debug.LogWarning("Barrel Purchase button had a null button or button text, make sure they arent null, name: " + gameObject.name + ", destroyed on runtime");
            Destroy(this.gameObject);
            return;
        }

        switch (buttonBarrelType)
        {
            case BarrelType.SMG:
                cost = StaticGunData.instance.SMGPrefabs[0].GetComponent<GunBarrel>().purchasePrice;
                break;
            case BarrelType.Pistol:
                cost = StaticGunData.instance.PistolPrefabs[0].GetComponent<GunBarrel>().purchasePrice;
                break;
            case BarrelType.Shotgun:
                cost = StaticGunData.instance.ShotGunPrefabs[0].GetComponent<GunBarrel>().purchasePrice;
                break;
            case BarrelType.MachineGun:
                cost = StaticGunData.instance.MachineGunPrefabs[0].GetComponent<GunBarrel>().purchasePrice;
                break;
            case BarrelType.Sniper:
                cost = StaticGunData.instance.SniperPrefabs[0].GetComponent<GunBarrel>().purchasePrice;
                break;
            case BarrelType.RocketLauncher:
                cost = StaticGunData.instance.RocketLauncherPrefabs[0].GetComponent<GunBarrel>().purchasePrice;
                break;
            case BarrelType.Empty:
                break;
            default:
                break;
        }

        buttonText.text = ("Create " + gunName + " <b> <size=17> " + cost.ToString() + "</size> </b>");
    }

    private void OnEnable()
    {
        UIEvents.instance.onCheckCosts += CheckIfCanAfford;
    }

    private void OnDisable()
    {
        UIEvents.instance.onCheckCosts -= CheckIfCanAfford;
    }

    public void CheckIfCanAfford()
    {
        if (cost > PlayerResourcesManager.scrap)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void HideOnInventoryFull()
    {

    }
}
