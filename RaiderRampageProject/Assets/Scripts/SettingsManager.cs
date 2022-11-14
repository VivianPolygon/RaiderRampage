using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//keeps tracks of settings information. dosent destroy on load so it follows from scene to scene

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public static float aimSensitivity;

    public static bool leftHandMode;

    public static Toggle leftHandToggle;

    private static PlayerData _saveData;

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


    //controls if the levels aiming canvas will be oriented to left handed players
    //Mirrors Bottom UI to allow left handers to aim with their dominant thumb (your welcome Dan)
    public static void SetLeftHandMode(bool state)
    {
        leftHandMode = state;
    }

    public static void SwapLeftHandMode()
    {
        if(leftHandMode)
        {
            leftHandMode = false;
            return;
        }
        else
        {
            leftHandMode = true;
        }
    }

    //sets the sensitivity value, clamps between 1 and 10 (coresponds to the slider that sets it) clamped for safter incase its accidentlaly called wrong
    public static void SetAimSensitivity(float newSensitivity)
    {
        //warns if the value needed to be clamped
        if(newSensitivity > 10 || newSensitivity < 0)
        {
            Debug.LogWarning("the aim sensitivty on SettingsManager was attempted to be set either above 10, or below 1. it has been clamped. It was trying to be set to: " + newSensitivity);
        }

        aimSensitivity = Mathf.Clamp(newSensitivity, 1, 10) / 10; //divides by 10. a sensitivity of 1 reduces speed by 90% a sensitivity of 5 by 50% and 10 by 0%
    }

    public static void LoadSettingsData()
    {
        _saveData = SaveManager.LoadPlayerData();

        if(_saveData != null)
        {
            aimSensitivity = _saveData.aimSensitivity;
            leftHandMode = _saveData.leftHanded;
        }

        InitilizeToggle();
    }

    public static void DefaultSettings(int sensitivity, bool leftHand)
    {
        SetAimSensitivity(sensitivity);
        SetLeftHandMode(leftHand);
    }

    public static void SetToggleLeftHandMode()
    {
        if (leftHandToggle.isOn)
        {
            SetLeftHandMode(true);
        }
        else
        {
            SetLeftHandMode(false);
        }
    }

    public static void InitilizeToggle()
    {
        print(leftHandMode);
        if (leftHandMode)
        {
            leftHandToggle.isOn = true;
            return;
        }
        else
        {
            leftHandToggle.isOn = false;
            return;
        }
    }
}
