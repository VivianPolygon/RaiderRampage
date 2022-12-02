using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WaveTracker : MonoBehaviour
{
    public static WaveTracker instance;
    public WaveSpawner waveSpawner;

    [Header("LevelWaveData")]
    [SerializeField]
    private WaveScriptableObject[] waves;

    [HideInInspector]
    public int currentWave = 0;

    [Header("Audio Stuff")]
    public AudioSource waveStartSong;

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
        //plays wave song
        StartWaveSong();

        waveSpawner.SetWaveData(waves[currentWave], gameObject);
        waveSpawner.SpawnWave();
        UIData.instance.SetWaveText(currentWave + 1);

        currentWave++;
    }

    public void CheckVictory()
    {
        if (currentWave >= waves.Length)
        {
            UIData.instance.VictoryScreen();
        }
    }

    public void StartWaveSong()
    {
        if (waveStartSong != null)
        {
            waveStartSong.time = 0;
            waveStartSong.Play();
        }
    }
    public void StopWaveSong()
    {
        if (waveStartSong != null)
        {
            waveStartSong.Pause(); 
        }
    }
}
