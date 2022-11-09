using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps tracks of settings information. dosent destroy on load so it follows from scene to scene

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public static float aimSensitivity;

    public static bool leftHandMode;

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

        aimSensitivity = _saveData.aimSensitivity;
        leftHandMode = _saveData.leftHanded;
    }
}
