using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    //level progress
    public int highestLevelBeat;

    //Settings
    public float aimSensitivity;
    public bool leftHanded;

    public float musicVolume;
    public float SFXVolume;

    public PlayerData(ProgressManager progress)
    {
        highestLevelBeat = progress.highestLevelCompleted;

        aimSensitivity = SettingsManager.aimSensitivity;
        leftHanded = SettingsManager.leftHandMode;

        musicVolume = AudioManager.musicVolume;
        SFXVolume = AudioManager.SFXVolume;
    }

}
