using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreenRagdollSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] ragdollPrefabs;

    [SerializeField] private int ragdollForceMin = 3000;
    [SerializeField] private int ragdollForceMax = 3000;

    [SerializeField] private float ragdollDestroyTime = 25;
    [SerializeField] private float ragdollSpawnTime = 0.5f;

    [SerializeField] private Vector3 randomRotationRange;


    private GameObject spawnedDoll;

    [SerializeField] private GameObject grenadePrefab;

    [SerializeField] private float grenadeExplodeTime;

    private GameObject spawnedGrenade;

    private void Start()
    {
        InvokeRepeating("FlingRagdoll", ragdollSpawnTime, ragdollSpawnTime);
        InvokeRepeating("FlingGrenades", 1, 1);
    }

    private void FlingRagdoll()
    {
        spawnedDoll = Instantiate(ragdollPrefabs[Random.Range(0, ragdollPrefabs.Length)], transform.position, transform.rotation);

        spawnedDoll.transform.Rotate(Random.Range(-randomRotationRange.x, randomRotationRange.x), Random.Range(-randomRotationRange.y, randomRotationRange.y), Random.Range(-randomRotationRange.z, randomRotationRange.z));

        foreach (Transform subTransform in spawnedDoll.transform)
        {
            if(subTransform.TryGetComponent(out Rigidbody rBody))
            {
                rBody.AddForce(spawnedDoll.transform.forward * Random.Range(ragdollForceMin, ragdollForceMax), ForceMode.Impulse);
            }
        }


        Destroy(spawnedDoll, ragdollDestroyTime);
    }

    private void FlingGrenades()
    {
        spawnedGrenade = Instantiate(grenadePrefab, transform.position, transform.rotation);

        spawnedGrenade.transform.Rotate(Random.Range(-randomRotationRange.x, randomRotationRange.x), Random.Range(-randomRotationRange.y, randomRotationRange.y), Random.Range(-randomRotationRange.z, randomRotationRange.z));

        if(spawnedGrenade.TryGetComponent(out Rigidbody rBody))
        {
            rBody.AddForce(spawnedDoll.transform.forward * Random.Range(ragdollForceMin, ragdollForceMax), ForceMode.Impulse);
        }

        Destroy(spawnedGrenade, grenadeExplodeTime);
    }

}
