using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance;


    public int highestLevelCompleted;


    private PlayerData playerData;

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
