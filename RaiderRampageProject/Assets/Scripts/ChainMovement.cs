using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//in tandom with the chain shader, animated the chain on the gun
public class ChainMovement : MonoBehaviour
{
    [SerializeField] private float chainSpeedFactor;
    public static float chainSpeed;

    [SerializeField] private Material chainMat;

    private float totalMovement = 0;


    public void Start()
    {
        UpdateChainSpeed(0);
    }

    private void Update()
    {
        SpinChain();
    }


    public void SpinChain()
    {
        totalMovement += (Time.deltaTime * (chainSpeed * chainSpeedFactor));
        chainMat.SetFloat("_ChainTotalMovement", totalMovement);
    }

    public static void UpdateChainSpeed(float newSpeed)
    {
        chainSpeed = newSpeed;
    }
}
