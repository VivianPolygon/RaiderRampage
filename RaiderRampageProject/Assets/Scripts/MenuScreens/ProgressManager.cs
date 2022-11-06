using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static int highestLevelCompleted;

    public int testingvalue;
    private void Start()
    {
        highestLevelCompleted = testingvalue;
    }
}
