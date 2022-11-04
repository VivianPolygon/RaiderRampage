using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grenade : MonoBehaviour
{
    [Header("Joint used to aim grenade")]
    [SerializeField] private Transform grenadeAimTransform;

    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform grenadeHoldingTransform;

    [SerializeField] private float grenadeExplosionDelay;



    //sets stuff on explosion script attatched to the prefab
    [Header("Explosion Stats")]
    [SerializeField] private int grenadeDamage;
    [SerializeField] private int grenadeRadius;
    [SerializeField] private int grenadeForce;

    [Header("For Explosion Effects")]
    [SerializeField] private GameObject grenadeParticleEffect;
    [SerializeField] private float effectTime;

    [Header("Throwing Variables")]
    [SerializeField] private float throwForce;
    [SerializeField] private float lobIntensity;


    private Image grenadeButtonImage; // pulled from UI Data


    private GameObject grenadeInstance;
    private Coroutine explosionTimeTracking;

    private bool hasGrenade;

    private void Awake()
    {
        CheckNull();
        SetUIComponents();
    }

    public void SpawnGrenade()
    {
        if (grenadeInstance == null && hasGrenade)
        {
            grenadeInstance = Instantiate(grenadePrefab, grenadeHoldingTransform.position, grenadeHoldingTransform.rotation);
            grenadeInstance.GetComponent<Rigidbody>().useGravity = false;
            SetExplosive(grenadeInstance.GetComponent<Explosive>());

            explosionTimeTracking = StartCoroutine(TrackGrenadeTime());


            Destroy(grenadeInstance, grenadeExplosionDelay);

            hasGrenade = false;
            StartCoroutine(GrenadeRefillTimer());
        }
    }

    public void ThrowGrenade()
    {
        if (grenadeInstance != null)
        {
            grenadeInstance.GetComponent<Rigidbody>().useGravity = true;
            grenadeInstance.GetComponent<Rigidbody>().AddForce((grenadeAimTransform.forward * throwForce) + (grenadeAimTransform.up * lobIntensity), ForceMode.Impulse);
            grenadeInstance = null;

            if(explosionTimeTracking != null)
            {
                StopCoroutine(explosionTimeTracking);
            }
        }     
    }

    private void SetUIComponents()
    {
        if(UIData.instance.grenadeImage != null)
        {
            grenadeButtonImage = UIData.instance.grenadeImage;
        }
        else
        {
            Debug.LogWarning("The Grenade script on: " + name + "does not have acsess to a grenade button image, most likely the UI data does not have one set, or is not present in the scene");
            Destroy(this);
        }
    }


    private void CheckNull()
    {
        if(grenadePrefab == null || grenadeHoldingTransform == null || grenadeParticleEffect == null)
        {
            Debug.LogWarning("Grenade script on: " + name + "Has a null variable, make sure to set them all in the inspector, Grenade script disabled to prevent errors");
            enabled = false;
        }
        if(grenadePrefab.GetComponent<Explosive>() == null)
        {
            Debug.LogWarning("Grenade prefab on Grenade script at: " + name + "Make sure the grenade prefab has an explosive script attatched, Grenade script disabled to prevent errors");
            enabled = false;
        }
        if (grenadePrefab.GetComponent<Rigidbody>() == null)
        {
            Debug.LogWarning("Grenade prefab on Grenade script at: " + name + "Make sure the grenade prefab has a rigidbody attatched, Grenade script disabled to prevent errors");
            enabled = false;
        }

        explosionTimeTracking = null;
        hasGrenade = true;
    }

    private void SetExplosive(Explosive explosive)
    {
        explosive.explosionDamage = grenadeDamage;
        explosive.explosionRange = grenadeRadius;
        explosive.explosionForce = grenadeForce;

        explosive.explosionVisual = grenadeParticleEffect;
        explosive.visualTime = effectTime;
    }

    private IEnumerator TrackGrenadeTime()
    {
        for (float i = 0; i < grenadeExplosionDelay; i += Time.deltaTime)
        {
            //throws if a second is left before exploding
            if (i + 1 >= grenadeExplosionDelay)
            {
                ThrowGrenade();
            }

                yield return null;
        }
    }

    private IEnumerator GrenadeRefillTimer()
    {
        for (float t = 0; t < PlayerResourcesManager.instance.grenadeRefillTime; t += Time.deltaTime)
        {
            grenadeButtonImage.fillAmount = t / PlayerResourcesManager.instance.grenadeRefillTime;
            yield return null;
        }

        grenadeButtonImage.fillAmount = 1;
        hasGrenade = true;
    }
}
