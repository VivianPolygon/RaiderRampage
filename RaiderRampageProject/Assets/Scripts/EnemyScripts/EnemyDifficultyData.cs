using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDifficultyData : MonoBehaviour
{
    public enum BiasType
    {
        Balanced,
        Agile,
        Armoured
    }


    [Header("Used to determine spawn ratios of available enemies")]
    [Header("Wave Spawning Data")]
    public int enemyDifficulty;
    [Header("Difficulty in Which This Enemy Will no Longer Appear")]
    public int enemyDifficultyCutoff;
    [Header("General Type of Enemy")]
    public BiasType enemyBias;
    [Header("Cost the enemy takes from the wave value")]
    public int enemyCost;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
