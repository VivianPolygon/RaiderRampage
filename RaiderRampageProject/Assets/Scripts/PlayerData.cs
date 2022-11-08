using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    //level progress
    public int highestLevelBeat;

    public PlayerData(ProgressManager progress)
    {
        highestLevelBeat = progress.highestLevelCompleted;
    }

}
