using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GunEffects : MonoBehaviour
{
    [Header("Object Should have Particle System")]

    private ParticleSystem smokeSystem;
    private ParticleSystem.EmissionModule smokeSystemEmission;

    private float rate;
    private bool isEnabled;

    [Header("For Muzzle Flash")]
    public Image flashImage;
    [SerializeField] private float flashDuration;


    [SerializeField] private Sprite[] flashSprites;

    private Coroutine flash;

    private void Start()
    {
        if (TryGetComponent(out ParticleSystem pSystem))
        {
            smokeSystem = pSystem;

            smokeSystemEmission = smokeSystem.emission;
            rate = smokeSystem.emission.rateOverTime.constant;
        }

        flash = null;
        flashImage.enabled = false;
        SetParticleSystemOff();
    }

    private void Update()
    {
        if (isEnabled)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }


    private void OnEnable()
    {
        GunData.instance.onBarrelSmoke += SetParticleSystemOn;
        GunData.instance.offBarrelSmoke += SetParticleSystemOff;
    }

    private void OnDisable()
    {
        GunData.instance.onBarrelSmoke -= SetParticleSystemOn;
        GunData.instance.offBarrelSmoke -= SetParticleSystemOff;
    }

    public void SetParticleSystemOn()
    {
        if (smokeSystem != null)
        {
            smokeSystemEmission.rateOverTime = rate;
            isEnabled = true;
        }
    }
    public void SetParticleSystemOff()
    {
        if (smokeSystem != null)
        {
            smokeSystemEmission.rateOverTime = 0;
            isEnabled = false;
        }
    }


    //causes the muzzle flash effect, called on GunBarrel
    public void FlashEffect()
    {
        //array of muzzle sprites on gundata, if your getting an out of bounds error from here, check the gundata script in the scene and make sure its array has atleast one Sprite
        flashImage.sprite = GunData.instance.muzzleFlashes[Random.Range(0, GunData.instance.muzzleFlashes.Length)];

        if(flash != null)
        {
            StopCoroutine(flash);
        }

        flash = StartCoroutine(MuzzleFlash());
    }


    private IEnumerator MuzzleFlash()
    {
        SetMuzzleSprite();
        flashImage.enabled = true;

        for (float i = 0; i < flashDuration; i += Time.deltaTime)
        {
            yield return null;
        }

        flashImage.enabled = false;
    }

    private void SetMuzzleSprite()
    {
        if(flashSprites != null)
        {
            flashImage.sprite = flashSprites[Random.Range(0, flashSprites.Length)];
        }
    }
}
