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

    [Header("Controls the barrels Smoke/Flash Effects")]
    [SerializeField] private GunEffects gunEffects;


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

    [Header("Audio Stuff")]
    [SerializeField] private AudioSource barrelAudioSource;

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
                    canFire = PlayerResourcesManager.instance.CheckCanShoot(0, ammoDrain);
                    break;
                case BarrelType.Pistol:
                    canFire = PlayerResourcesManager.instance.CheckCanShoot(0, ammoDrain);
                    break;

                case BarrelType.Shotgun:
                    canFire = PlayerResourcesManager.instance.CheckCanShoot(1, ammoDrain);
                    break;

                case BarrelType.MachineGun:
                    canFire = PlayerResourcesManager.instance.CheckCanShoot(2, ammoDrain);
                    break;
                case BarrelType.Sniper:
                    canFire = PlayerResourcesManager.instance.CheckCanShoot(2, ammoDrain);
                    break;

                case BarrelType.RocketLauncher:
                    canFire = PlayerResourcesManager.instance.CheckCanShoot(3, ammoDrain);
                    break;
                default:
                    break;

            }

            //tracks time, always tracks so that the player cant spam shot by quicktapping,
            //but dosent have to wait the full shot time if they havent shot in awhile
            t += Time.deltaTime;

            //if the time requirments have been met, instantiates shot and does ammo caculations
            if(t > (1 / (shotsPerSecond * PlayerResourcesManager.fireSpeedMult)) && canFire)
            {
                t = 0;
                Vector3 appliedSpread = Vector3.zero;

                //muzzleFlash
                gunEffects.FlashEffect();

                //sound effect
                PlayShotAudio();

                switch (bulletPattern)
                {

                    case BarrelShootingPattern.Standard:
                        shot = Instantiate(projectilePrefab, projectileSpawnpoint.transform.position, transform.rotation);
                        shot.transform.LookAt(GunData.instance.cursorPositon);
                        shot.GetComponent<Rigidbody>().AddForce(((shot.transform.forward * shotForce ) + (Random.onUnitSphere * bulletSpreadAmount) * GunData.instance.spinSpeedPercent), ForceMode.Impulse);

                        //does aditional behaviors on overdrive
                        if(OverdriveGauge.armorPierceActive)
                        {
                            shot.transform.localScale *= OverdriveGauge.pierceScale;
                        }
                        if(OverdriveGauge.incendiaryActive)
                        {
                            GameObject fireTrail = Instantiate(OverdriveGauge.fireTrail, shot.transform.position, shot.transform.rotation);
                            fireTrail.transform.parent = shot.transform;
                        }
                        if(OverdriveGauge.explosionActive)
                        {
                            GameObject explosionObject = Instantiate(OverdriveGauge.explosionObject, shot.transform.position, shot.transform.rotation);
                            explosionObject.transform.parent = shot.transform;

                            explosionObject.GetComponent<Explosive>().explosionDamage = OverdriveGauge._explosionDamage;
                            explosionObject.GetComponent<Explosive>().explosionRange = OverdriveGauge._explosionRange;
                        }

                        Destroy(shot, projectileDestroyTime);

                        break;
                    case BarrelShootingPattern.Spread:
                        for (int i = 0; i < spreadShotQuantity; i++)
                        {
                            shot = Instantiate(projectilePrefab, projectileSpawnpoint.transform.position, transform.rotation);
                            shot.transform.LookAt(GunData.instance.cursorPositon);
                            shot.GetComponent<Rigidbody>().AddForce((shot.transform.forward * shotForce)+ (Random.onUnitSphere * bulletSpreadAmount), ForceMode.Impulse);

                            //does aditional behaviors on overdrive
                            if (OverdriveGauge.armorPierceActive)
                            {
                                shot.transform.localScale *= OverdriveGauge.pierceScale;
                            }
                            if (OverdriveGauge.incendiaryActive)
                            {
                                GameObject fireTrail = Instantiate(OverdriveGauge.fireTrail, shot.transform.position, shot.transform.rotation);
                                fireTrail.transform.parent = shot.transform;
                            }
                            if (OverdriveGauge.explosionActive)
                            {
                                GameObject explosionObject = Instantiate(OverdriveGauge.explosionObject, shot.transform.position, shot.transform.rotation);
                                explosionObject.transform.parent = shot.transform;

                                explosionObject.GetComponent<Explosive>().explosionDamage = OverdriveGauge._explosionDamage;
                                explosionObject.GetComponent<Explosive>().explosionRange = OverdriveGauge._explosionRange;
                            }

                            Destroy(shot, projectileDestroyTime);
                        }

                        break;
                    default:
                        break;
                }


                
                switch (barrelType)
                {
                    case BarrelType.SMG:
                        PlayerResourcesManager.instance.clipQuantities[0] -= ammoDrain;
                        UIData.instance.UpdateSpecificClipDrainIcon(0);
                        break;
                    case BarrelType.Pistol:
                        PlayerResourcesManager.instance.clipQuantities[0] -= ammoDrain;
                        UIData.instance.UpdateSpecificClipDrainIcon(0);
                        break;

                    case BarrelType.Shotgun:
                        PlayerResourcesManager.instance.clipQuantities[1] -= ammoDrain;
                        UIData.instance.UpdateSpecificClipDrainIcon(1);
                        break;

                    case BarrelType.MachineGun:
                        PlayerResourcesManager.instance.clipQuantities[2] -= ammoDrain;
                        UIData.instance.UpdateSpecificClipDrainIcon(2);
                        break;
                    case BarrelType.Sniper:
                        PlayerResourcesManager.instance.clipQuantities[2] -= ammoDrain;
                        UIData.instance.UpdateSpecificClipDrainIcon(2);
                        break;

                    case BarrelType.RocketLauncher:
                        PlayerResourcesManager.instance.clipQuantities[3] -= ammoDrain;
                        UIData.instance.UpdateSpecificClipDrainIcon(3);
                        break;
                    default:
                        break;
                }
                
            }
            //sets false for next loop
            canFire = false;

        }
    }

    private void PlayShotAudio()
    {
        if(barrelAudioSource != null)
        {
            barrelAudioSource.time = 0;
            barrelAudioSource.Play();
        }
    }

}
