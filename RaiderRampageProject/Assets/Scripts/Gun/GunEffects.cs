using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunEffects : MonoBehaviour
{
    [Header("Object Should have Particle System")]

    private ParticleSystem smokeSystem;
    private ParticleSystem.EmissionModule smokeSystemEmission;

    private float rate;
    private bool isEnabled;

    private void Start()
    {
        if(TryGetComponent(out ParticleSystem pSystem))
        {
            smokeSystem = pSystem;

            smokeSystemEmission = smokeSystem.emission;
            rate = smokeSystem.emission.rateOverTime.constant;

        }

        SetParticleSystemOff();
    }

    private void Update()
    {
        if(isEnabled)
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
        if(smokeSystem != null)
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

}
