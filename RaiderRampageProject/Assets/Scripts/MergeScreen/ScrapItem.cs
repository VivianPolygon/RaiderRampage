using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapItem : MonoBehaviour
{ 
    public void ScrapBarrel(BarrelType type, BarrelTeir tier)
    {
        switch (type)
        {
            case BarrelType.SMG:
                if(StaticGunData.instance.SMGPrefabs[(int)tier].TryGetComponent(out GunBarrel smgBarrel))
                {
                    PlayerResourcesManager.scrap = Mathf.Clamp(PlayerResourcesManager.scrap + smgBarrel.scrapValue, 0, PlayerResourcesManager.instance.scrapCap);
                }                
                break;
            case BarrelType.Pistol:
                if (StaticGunData.instance.PistolPrefabs[(int)tier].TryGetComponent(out GunBarrel pistolBarrel))
                {
                    PlayerResourcesManager.scrap = Mathf.Clamp(PlayerResourcesManager.scrap + pistolBarrel.scrapValue, 0, PlayerResourcesManager.instance.scrapCap);
                }
                break;
            case BarrelType.Shotgun:
                if (StaticGunData.instance.ShotGunPrefabs[(int)tier].TryGetComponent(out GunBarrel shotgunBarrel))
                {
                    PlayerResourcesManager.scrap = Mathf.Clamp(PlayerResourcesManager.scrap + shotgunBarrel.scrapValue, 0, PlayerResourcesManager.instance.scrapCap);
                }
                break;
            case BarrelType.MachineGun:
                if (StaticGunData.instance.MachineGunPrefabs[(int)tier].TryGetComponent(out GunBarrel machinegunBarrel))
                {
                    PlayerResourcesManager.scrap = Mathf.Clamp(PlayerResourcesManager.scrap + machinegunBarrel.scrapValue, 0, PlayerResourcesManager.instance.scrapCap);
                }
                break;
            case BarrelType.Sniper:
                if (StaticGunData.instance.SniperPrefabs[(int)tier].TryGetComponent(out GunBarrel sniperBarrel))
                {
                    PlayerResourcesManager.scrap = Mathf.Clamp(PlayerResourcesManager.scrap + sniperBarrel.scrapValue, 0, PlayerResourcesManager.instance.scrapCap);
                }
                break;
            case BarrelType.RocketLauncher:
                if (StaticGunData.instance.RocketLauncherPrefabs[(int)tier].TryGetComponent(out GunBarrel rocketLauncherBarrel))
                {
                    PlayerResourcesManager.scrap = Mathf.Clamp(PlayerResourcesManager.scrap + rocketLauncherBarrel.scrapValue, 0, PlayerResourcesManager.instance.scrapCap);
                }
                break;
            case BarrelType.Empty:
                break;
            default:
                break;

        }

        UIEvents.instance.UpdateScrapCounts();
    }
}
