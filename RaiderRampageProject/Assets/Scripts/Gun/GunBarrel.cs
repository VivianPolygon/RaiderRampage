using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that holds information and controls the behavior of a gun barrel
public class GunBarrel : MonoBehaviour
{
    //used to determine what type of barrel this is, used for ammo caculations
    public BarrelType barrelType;
    public BarrelTeir barrelTier;

    public BarrelShootingPattern bulletPattern;


    //prefab for projectile, and its spawnpoint transform
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnpoint;

    //how quickly the barrel shoots
    [SerializeField]
    private float shotsPerSecond;
    //how much ammo each shot drains from its pool
    [SerializeField]
    private int ammoDrain;
    //impulse physics force applied to the bullet on spawn
    [SerializeField]
    private float shotForce;
    [SerializeField]
    private int bulletDamage;
    //amount of scrap regained when discarded
    public int scrapValue;


    //how long until the projectile gets destroyed if it collides with nothing
    [SerializeField]
    private float projectileDestroyTime;

    //variables used in caculations, time, current bullet, and flag for it the barrel can fire
    private float t;
    private GameObject shot;
    private bool canFire;

    //variables used to control how the bullets shoot
    [SerializeField]
    private float bulletSpreadAmount;
    [Header("only applyed if bulletPattern is set to spread")]
    [SerializeField]
    private int spreadShotQuantity;

    [Header("cost to purchase on the UpgradeScreen")]
    public int purchasePrice;

    private void Awake()
    {
        //initilizes time and can fire flag
        t = 0;
        canFire = false;
    }

    private void Start()
    {
        if (projectilePrefab.TryGetComponent(out Bullet bullet))
        {
            bullet.damage = bulletDamage;
        }
        if (projectilePrefab.TryGetComponent(out Explosive explosive))
        {
            explosive.explosionDamage = bulletDamage;
        }
    }

    void Update()
    {
        //if the fire button is held, checks ammo stocks to determine if the barrel can fire
        if(GunData.instance.firing && !GunData.instance.reloading)
        {
            switch (barrelType)
            {
                case BarrelType.SMG:
                    if (PlayerResourcesManager.instance.pistolClipCurrent >= ammoDrain)
                    {
                        canFire = true;
                    }
                    break;
                case BarrelType.Pistol:
                    if(PlayerResourcesManager.instance.pistolClipCurrent >= ammoDrain)
                    {
                        canFire = true;
                    }
                    break;

                case BarrelType.Shotgun:
                    if (PlayerResourcesManager.instance.shotGunClipCurrent >= ammoDrain)
                    {
                        canFire = true;
                    }
                    break;

                case BarrelType.MachineGun:
                    if (PlayerResourcesManager.instance.machineGunClipCurrent >= ammoDrain)
                    {
                        canFire = true;
                    }
                    break;
                case BarrelType.Sniper:
                    if (PlayerResourcesManager.instance.machineGunClipCurrent >= ammoDrain)
                    {
                        canFire = true;
                    }
                    break;

                case BarrelType.RocketLauncher:
                    if (PlayerResourcesManager.instance.rocketLauncherClipCurrent >= ammoDrain)
                    {
                        canFire = true;
                    }
                    break;
                default:
                    break;
            }

            //tracks time, always tracks so that the player cant spam shot by quicktapping,
            //but dosent have to wait the full shot time if they havent shot in awhile
            t += Time.deltaTime;

            //if the time requirments have been met, instantiates shot and does ammo caculations
            if(t > (1 / shotsPerSecond) && canFire)
            {
                t = 0;
                Vector3 appliedSpread = Vector3.zero;
                switch (bulletPattern)
                {

                    case BarrelShootingPattern.Standard:
                        shot = Instantiate(projectilePrefab, projectileSpawnpoint.transform.position, transform.rotation);
                        shot.transform.LookAt(GunData.instance.cursorPositon);
                        shot.GetComponent<Rigidbody>().AddForce(((shot.transform.forward * shotForce ) + (Random.onUnitSphere * bulletSpreadAmount) * GunData.instance.spinSpeedPercent), ForceMode.Impulse);

                        Destroy(shot, projectileDestroyTime);

                        break;
                    case BarrelShootingPattern.Spread:
                        for (int i = 0; i < spreadShotQuantity; i++)
                        {
                            shot = Instantiate(projectilePrefab, projectileSpawnpoint.transform.position, transform.rotation);
                            shot.transform.LookAt(GunData.instance.cursorPositon);
                            shot.GetComponent<Rigidbody>().AddForce((shot.transform.forward * shotForce)+ (Random.onUnitSphere * bulletSpreadAmount), ForceMode.Impulse);
                            Destroy(shot, projectileDestroyTime);
                        }

                        break;
                    default:
                        break;
                }


                
                switch (barrelType)
                {
                    case BarrelType.SMG:
                        PlayerResourcesManager.instance.pistolClipCurrent -= ammoDrain;
                        UIData.instance.UpdateAmmoSlider(4, PlayerResourcesManager.instance.pistolClipMax, PlayerResourcesManager.instance.pistolClipCurrent);
                        break;
                    case BarrelType.Pistol:
                        PlayerResourcesManager.instance.pistolClipCurrent -= ammoDrain;
                        UIData.instance.UpdateAmmoSlider(4, PlayerResourcesManager.instance.pistolClipMax, PlayerResourcesManager.instance.pistolClipCurrent);
                        break;

                    case BarrelType.Shotgun:
                        PlayerResourcesManager.instance.shotGunClipCurrent -= ammoDrain;
                        UIData.instance.UpdateAmmoSlider(6, PlayerResourcesManager.instance.shotGunClipMax, PlayerResourcesManager.instance.shotGunClipCurrent);
                        break;

                    case BarrelType.MachineGun:
                        PlayerResourcesManager.instance.machineGunClipCurrent -= ammoDrain;
                        UIData.instance.UpdateAmmoSlider(5, PlayerResourcesManager.instance.machineGunClipMax, PlayerResourcesManager.instance.machineGunClipCurrent);
                        break;
                    case BarrelType.Sniper:
                        PlayerResourcesManager.instance.machineGunClipCurrent -= ammoDrain;
                        UIData.instance.UpdateAmmoSlider(5, PlayerResourcesManager.instance.machineGunClipMax, PlayerResourcesManager.instance.machineGunClipCurrent);
                        break;

                    case BarrelType.RocketLauncher:
                        PlayerResourcesManager.instance.rocketLauncherClipCurrent -= ammoDrain;
                        UIData.instance.UpdateAmmoSlider(7, PlayerResourcesManager.instance.rocketLauncherClipMax, PlayerResourcesManager.instance.rocketLauncherClipCurrent);
                        break;
                    default:
                        break;
                }
                
            }
            //sets false for next loop
            canFire = false;
        }
    }

    private Vector3 RandomTargetFromSpread(Vector3 targetPosition)
    {
        Vector3 RandomVector;

        RandomVector.x = UnityEngine.Random.Range(targetPosition.x - bulletSpreadAmount, targetPosition.x + bulletSpreadAmount);
        RandomVector.y = UnityEngine.Random.Range(targetPosition.y - bulletSpreadAmount, targetPosition.y + bulletSpreadAmount);
        RandomVector.z = UnityEngine.Random.Range(targetPosition.z - bulletSpreadAmount, targetPosition.z + bulletSpreadAmount);

        return RandomVector;
    }
}