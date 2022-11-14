using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script holds data needed for stuff like the save system.
//curently holds:
//level number for tracking progress
//
//
public class LevelData : MonoBehaviour
{

    //used to update the levels completed tracking
    [SerializeField] private int levelNumber;
    public static int adjustedLevelNumber; //the level humber + 1. needed for checking the level properly

    private void Awake()
    {
        adjustedLevelNumber = levelNumber + 1;
    }

}
