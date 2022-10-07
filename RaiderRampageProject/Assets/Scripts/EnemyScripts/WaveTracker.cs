using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WaveTracker : MonoBehaviour
{
    public static WaveTracker instance;
    public WaveSpawner waveSpawner;

    [Header("LevelWaveData")]
    [SerializeField]
    private WaveScriptableObject[] waves;

    [HideInInspector]
    public int currentWave = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //subscription for game state manager
        GameStateManager.instance.onWaveStart += StartWave;
    }

    public void StartWave()
    {
        waveSpawner.SetWaveData(waves[currentWave], gameObject);
        waveSpawner.SpawnWave();
        UIData.instance.SetWaveText(currentWave + 1);
        currentWave = Mathf.Clamp(currentWave + 1, 0, waves.Length - 1);
    }

}
