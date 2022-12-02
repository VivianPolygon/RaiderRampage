using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibExplosion : MonoBehaviour
{
    [SerializeField] private GameObject[] gibChunckPrefabs;

    [SerializeField] private float gibSpawnRadius;

    [SerializeField] private int gibChunkSpawnAmountMin;
    [SerializeField] private int gibChunkSpawnAmountMax;

    [SerializeField] private float gibOutwardForce;
    [SerializeField] private float gibForceFalloffRate;

    [SerializeField] private float gibChunckScale;

    [SerializeField] private float gibDespawnTime;

    // Start is called before the first frame update
    void Start()
    {
        SpawnLaunchGibs();
    }

    private void SpawnLaunchGibs()
    {
        int gibAmount = Random.Range(gibChunkSpawnAmountMin, gibChunkSpawnAmountMax + 1);

        GameObject currentGib;
        float forceAmount;

        for (int i = 0; i < gibAmount; i++)
        {
            //instantiates the gib chunck
            currentGib = Instantiate(gibChunckPrefabs[Random.Range(0, gibChunckPrefabs.Length)], transform.position + (Random.insideUnitSphere * gibSpawnRadius), Random.rotation);
            currentGib.transform.parent = transform;

            currentGib.transform.localScale = Vector3.one * gibChunckScale;

            //trys to get the rigid body and apply appropiate force
            if(currentGib.TryGetComponent(out Rigidbody rBody))
            {
                forceAmount = gibOutwardForce / (Vector3.Distance(currentGib.transform.position, transform.position) / gibSpawnRadius) * gibForceFalloffRate;

                rBody.AddForce((currentGib.transform.position - transform.position).normalized * forceAmount, ForceMode.Impulse);
            }
        }

        Destroy(gameObject, gibDespawnTime);
    }
}
