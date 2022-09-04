using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBarrel : MonoBehaviour
{
    [SerializeField]
    BarrelType barrelType;

    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnpoint;

    [SerializeField]
    private float shotsPerSecond;
    [SerializeField]
    private int ammoDrain;
    [SerializeField]
    private float shotForce;

    [SerializeField]
    private float projectileDestroyTime;

    private float t;
    private GameObject shot;
    private bool canFire;

    private void Awake()
    {
        t = 0;
        canFire = false;
    }
    void Update()
    {
        if(GunData.instance.firing)
        {
            switch (barrelType)
            {
                case BarrelType.Pistol:
                    if(PlayerResourcesManager.instance.pistolAmmoCalc > ammoDrain)
                    {
                        canFire = true;
                    }
                    break;
                case BarrelType.Shotgun:
                    if (PlayerResourcesManager.instance.shotGunAmmoCalc > ammoDrain)
                    {
                        canFire = true;
                    }
                    break;
                case BarrelType.MachineGun:
                    if (PlayerResourcesManager.instance.machineGunAmmoCalc > ammoDrain)
                    {
                        canFire = true;
                    }
                    break;
                case BarrelType.RocketLauncher:
                    if (PlayerResourcesManager.instance.rocketLauncherAmmoCalc > ammoDrain)
                    {
                        canFire = true;
                    }
                    break;
                default:
                    break;
            }


            t += Time.deltaTime;
            if(t > (1 / shotsPerSecond) && canFire)
            {
                t = 0;
                shot = Instantiate(projectilePrefab, projectileSpawnpoint.transform.position, transform.rotation);
                shot.GetComponent<Rigidbody>().AddForce(shot.transform.forward * shotForce, ForceMode.Impulse);
                Destroy(shot, projectileDestroyTime);

                
                switch (barrelType)
                {
                    case BarrelType.Pistol:
                        PlayerResourcesManager.instance.pistolAmmoCalc -= ammoDrain;
                        break;
                    case BarrelType.Shotgun:
                        PlayerResourcesManager.instance.shotGunAmmoCalc -= ammoDrain;
                        break;
                    case BarrelType.MachineGun:
                        PlayerResourcesManager.instance.machineGunAmmoCalc -= ammoDrain;
                        break;
                    case BarrelType.RocketLauncher:
                        PlayerResourcesManager.instance.rocketLauncherAmmoCalc -= ammoDrain;
                        break;
                    default:
                        break;
                }
                
            }

            canFire = false;
        }
    }


}
