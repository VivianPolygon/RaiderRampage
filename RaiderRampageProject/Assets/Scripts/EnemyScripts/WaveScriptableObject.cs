using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WaveScriptableObject", menuName = "WaveData")]
public class WaveScriptableObject : ScriptableObject
{
    [Header("Wave Difficulty Info")]
    public int waveDifficultyThreshold = 15;
    public int waveDifficulty = 10;

    [Header("Enemy Type Bias, 1 - 100")]
    public int balancedBias = 50;
    public int fastBias = 50;
    public int armouredBias = 50;

    [Header("Wave Enemy Quantity")]
    public int waveValue = 100;
    public int waveQuantity = 30;

    [Header("Time between Enemy Spawns")]
    public float delayMin = 0.25f;
    public float delayMax = 1f;
}
