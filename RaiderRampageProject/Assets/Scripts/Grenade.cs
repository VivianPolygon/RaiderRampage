using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
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

    private GameObject grenadeInstance;
    private Coroutine explosionTimeTracking;

    private void Awake()
    {
        CheckNull();
    }

    public void SpawnGrenade()
    {
        if (grenadeInstance == null && explosionTimeTracking == null)
        {
            grenadeInstance = Instantiate(grenadePrefab, grenadeHoldingTransform.position, grenadeHoldingTransform.rotation);
            grenadeInstance.GetComponent<Rigidbody>().useGravity = false;
            SetExplosive(grenadeInstance.GetComponent<Explosive>());

            explosionTimeTracking = StartCoroutine(TrackGrenadeTime());


            Destroy(grenadeInstance, grenadeExplosionDelay);
        }
    }

    public void ThrowGrenade()
    {
        if(grenadeInstance != null)
        {
            grenadeInstance.GetComponent<Rigidbody>().useGravity = true;
            grenadeInstance.GetComponent<Rigidbody>().AddForce((transform.forward * throwForce) + (transform.up * lobIntensity), ForceMode.Impulse);
            grenadeInstance = null;

            if(explosionTimeTracking != null)
            {
                StopCoroutine(explosionTimeTracking);
            }
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
}
