using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance;


    public int highestLevelCompleted;


    private PlayerData playerData;

    //event for updating some UI when loaded, currently updates the audio sliders
    public event Action onLoadTriggered;
    public void LoadTriggered() { onLoadTriggered?.Invoke(); }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        highestLevelCompleted = Mathf.Clamp(highestLevelCompleted, 1, 100); //clamps the progress to 1, to prevent the player from ever potentialy being locked out of any levels by some wierd save issue
    }

    //instanced saveing functions

    public void Save()
    {
        SaveManager.SavePlayerData(this);
    }

    public void Load()
    {
        playerData = SaveManager.LoadPlayerData();
        if(playerData != null)
        {
            //sets the level progress
            highestLevelCompleted = playerData.highestLevelBeat;

            //sets the audio values
            AudioManager.SetMusicVolume(playerData.musicVolume);
            AudioManager.SetSFXVolume(playerData.SFXVolume);

            AudioManager.musicVolume = playerData.musicVolume;
            AudioManager.SFXVolume = playerData.SFXVolume;

            AudioManager.UpdateAudioValues();

            //calls update event
            LoadTriggered();


        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
    }

    public void DefaultProgress(int level)
    {
        highestLevelCompleted = level;
    }

    public void DeleteSave()
    {
        SaveManager.DeleteSaveData();
    }

}
