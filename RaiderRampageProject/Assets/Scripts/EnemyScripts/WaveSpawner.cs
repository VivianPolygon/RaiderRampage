using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    //updated threshold value of the wave
    [Header("Controls Strongest Enemies Spawned, Clamped between 1-100")]
    [SerializeField]
    private int waveDifficultyThreshold;
    //hidden ints that cap the threshold value
    private int waveThresholdMin = 0;
    private int waveThresholdMax = 100;

    //list of possible spawns from the threshold
    [SerializeField]
    private List<GameObject> possibleSpawns;


    [Header("Biases for Wave Enemy Types, Clamped between 1 and 100")]
    //ints used to caculate rations for each type to spawn from threshold list
    [SerializeField]
    private int balancedBias;
    [SerializeField]
    private int fastBias;
    [SerializeField]
    private int armouredBias;
    //hidden ints used for clamping
    private int biasMin = 1;
    private int biasMax = 100;
    //hidden floats used for bias percents
    private float balancedBiasPercent = 1 / 3;
    private float fastBiasPercent = 1 / 3;
    private float armouredBiasPercent = 1 / 3;
    //hidden ints used to caculate total amount of each type available
    private int balancedEnemiesAvailable;
    private int fastEnemiesAvailable;
    private int armouredEnemiesAvailable;



    [Header("Wave Difficulty, Determines Ratio of Stronger Enemies Spawned"), Space(5), Header("If an Enemy's Difficulty is Below this value, They Will No Longer Spawn, Clamped between 1 and 100")]
    [SerializeField]
    private int waveDifficulty;
    //hiddin ints that control minimum and max values for difficulty clamping
    private int difficultyMin = 1;
    private int difficultyMax = 100;


    [Header("Total Value of Enemies that Can be Spawned")]
    [SerializeField]
    private int waveMaxValue;
    [Header("Total Quantity of Enemies that Can be Spawned")]
    [SerializeField]
    private int waveMaxQuantity;

    [SerializeField]
    private List<GameObject> enemiesToSpawn;

    [Header("Random Time Delay Between Spawns")]
    //floats that control delay time between each enemy spawn, time is random between these two values
    [SerializeField]
    private float spawnDelayMin = 0.25f;
    [SerializeField]
    private float spawnDelayMax = 2f;

    //1. get possible spawn points for enemies - Done
    //2. figure possible enemy spawns from enemy difficulty threshold - Done
    //3. figure out ratios of types to spawn depending on wave bias - Done
    //4. figure out ratio of enemy difficulties to spawn depending on wave difficulty - Done
    //5. figure out time range between each spawn - Done
    //6. spawn enemies with above parameters - Done

    [Header("All Enemy Prefabs")]
    [SerializeField]
    private GameObject[] enemyPrefabs;

    //spawnpoint for the enemies, should be a node used for navigation
    [SerializeField]
    private Transform[] SpawnPoints;


    //set it up to allow parameters to be passed through
    public void SpawnWave()
    {
        SetWaveThreshold(waveDifficultyThreshold);
        SetBias(balancedBias, fastBias, armouredBias);
        SetDifficulty(waveDifficulty);
        SetWaveValues(waveMaxValue, waveMaxQuantity);
        SetSpawnList();
    }

    private void SetWaveThreshold(int newThreshhold)
    {
        //clamps the new wave threshold to the appropiate values
        waveDifficultyThreshold = Mathf.Clamp(newThreshhold, waveThresholdMin, waveThresholdMax);

        //initilized the enemy type counts to zero so they increment properly
        balancedEnemiesAvailable = 0;
        fastEnemiesAvailable = 0;
        armouredEnemiesAvailable = 0;

        //clears the list of possible spawns from the previous wave
        if(possibleSpawns != null && possibleSpawns.Count > 0)
        {
            possibleSpawns.Clear();
        }

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if(enemyPrefabs[i].TryGetComponent(out EnemyDifficultyData enemyData))
            {
                if(enemyData.enemyDifficulty <= waveDifficultyThreshold)
                {
                    possibleSpawns.Add(enemyData.gameObject);
                }

                switch (enemyData.enemyBias)
                {
                    case EnemyDifficultyData.BiasType.Balanced:
                        balancedEnemiesAvailable++;
                        break;
                    case EnemyDifficultyData.BiasType.Agile:
                        fastEnemiesAvailable++;
                        break;
                    case EnemyDifficultyData.BiasType.Armoured:
                        armouredEnemiesAvailable++;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Make sure every gameobject on the Wavespawner script's enemy prefab array located on " + name + 
                    "is a enemy prefab with the appropiate scripts, including EnemyDifficultyData");
            }
        }
    }


    private void SetBias(int newBalancedBias, int newFastBias, int newArmouredBias)
    {
        float biasTotal;

        balancedBias = Mathf.Clamp(newBalancedBias, biasMin, biasMax);
        fastBias = Mathf.Clamp(newFastBias, biasMin, biasMax);
        armouredBias = Mathf.Clamp(newArmouredBias, biasMin, biasMax);

        biasTotal = balancedBias + fastBias + armouredBias;

        balancedBiasPercent = (balancedBias / biasTotal) / balancedEnemiesAvailable;
        fastBiasPercent = (fastBias / biasTotal) / fastEnemiesAvailable;
        armouredBiasPercent = (armouredBias / biasTotal) / armouredEnemiesAvailable;
    }

    private void SetDifficulty(int newDifficulty)
    {
        waveDifficulty = Mathf.Clamp(newDifficulty, difficultyMin, difficultyMax);
    }

    private void SetWaveValues(int maxValue, int maxCount)
    {
        waveMaxValue = maxValue;
        waveMaxQuantity = maxCount;
    }

    private void SetSpawnList()
    {
        //clears the list of enemies from the previous wave
        if (enemiesToSpawn != null && enemiesToSpawn.Count > 0)
        {
            enemiesToSpawn.Clear();
        }
        StartCoroutine(FillEnemyList());
    }

    private IEnumerator FillEnemyList()
    {
        int currentValue = 0;
        int currentQuantity = 0;

        float percentChecker = Random.Range(0.01f, 0.99f);

        EnemyDifficultyData enemyData;
        while(currentValue < waveMaxValue && currentQuantity < waveMaxQuantity)
        {
            foreach(GameObject enemy in possibleSpawns)
            {
                enemyData = enemy.GetComponent<EnemyDifficultyData>();

                switch (enemyData.enemyBias)
                {
                    case EnemyDifficultyData.BiasType.Balanced:
                        percentChecker = Random.Range(0f, 1f);
                        if (percentChecker < balancedBiasPercent)
                        {
                            percentChecker = Random.Range(0f, waveDifficulty);
                            if(enemyData.enemyDifficulty <= percentChecker && enemyData.enemyDifficultyCutoff > waveDifficulty)
                            {
                                currentValue += enemyData.enemyCost;
                                currentQuantity++;

                                enemiesToSpawn.Add(enemyData.gameObject);
                            }
                        }
                        break;
                    case EnemyDifficultyData.BiasType.Agile:
                        percentChecker = Random.Range(0f, 1f);
                        if (percentChecker < fastBiasPercent)
                        {
                            percentChecker = Random.Range(0f, waveDifficulty);
                            if (enemyData.enemyDifficulty <= percentChecker && enemyData.enemyDifficultyCutoff > waveDifficulty)
                            {
                                currentValue += enemyData.enemyCost;
                                currentQuantity++;

                                enemiesToSpawn.Add(enemyData.gameObject);
                            }
                        }
                        break;
                    case EnemyDifficultyData.BiasType.Armoured:
                        percentChecker = Random.Range(0f, 1f);
                        if (percentChecker < armouredBiasPercent)
                        {
                            percentChecker = Random.Range(0f, waveDifficulty);
                            if (enemyData.enemyDifficulty <= percentChecker && enemyData.enemyDifficultyCutoff > waveDifficulty)
                            {
                                currentValue += enemyData.enemyCost;
                                currentQuantity++;

                                enemiesToSpawn.Add(enemyData.gameObject);
                            }
                        }
                        break;
                    default:
                        break;
                }
                yield return null;
            }
            yield return null;
        }

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            Instantiate(enemiesToSpawn[i], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, transform.rotation);

            yield return new WaitForSecondsRealtime(Random.Range(spawnDelayMin, spawnDelayMax));
        }
    }


    //used for testing sliders on UI, testing only <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    public void SetBalanceBias(int value)
    {
        balancedBias = Mathf.Clamp(value, biasMin, biasMax);
    }
    public void SetFastBias(int value)
    {
        fastBias = Mathf.Clamp(value, biasMin, biasMax);
    }
    public void SetArmouredBias(int value)
    {
        armouredBias = Mathf.Clamp(value, biasMin, biasMax);
    }

}
