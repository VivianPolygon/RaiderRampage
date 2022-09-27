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

    [SerializeField]
    private GameObject WaveButton;

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


    private void Update()
    {
        if(WaveSpawner.waveActive)
        { 
            if(WaveButton.activeSelf)
            {
                WaveButton.SetActive(false);
            }
        }
        else
        {
            if(!WaveButton.activeSelf)
            {
                WaveButton.SetActive(true);
            }
        }
    }

    public void StartWave()
    {
        waveSpawner.SetWaveData(waves[currentWave], gameObject);
        waveSpawner.SpawnWave();
        UIData.instance.SetWaveText(currentWave + 1);
        currentWave = Mathf.Clamp(currentWave + 1, 0, waves.Length - 1);
    }


}
