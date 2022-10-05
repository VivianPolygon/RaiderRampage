using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    //updated threshold value of the wave
    private int waveDifficultyThreshold;
    //hidden ints that cap the threshold value
    private int waveThresholdMin = 0;
    private int waveThresholdMax = 100;

    //ints used to caculate rations for each type to spawn from threshold list
    private int balancedBias;
    private int fastBias;
    private int armouredBias;
    //hidden ints used for clamping
    private int biasMin = 1;
    private int biasMax = 100;
    //hidden floats used for bias percents
    private float balancedBiasPercent = 1 / 3;
    private float fastBiasPercent = 1 / 3;
    private float armouredBiasPercent = 1 / 3;

    private int waveDifficulty;
    //hiddin ints that control minimum and max values for difficulty clamping
    private int difficultyMin = 1;
    private int difficultyMax = 100;

    private int waveMaxValue;
    private int waveMaxQuantity;

    //list of possible spawns from the difficulty
    private List<GameObject> possibleSpawns;
    //list of enemies that will be looped through and spawned
    private List<GameObject> enemiesToSpawn;
    //list of enemies of the generated type that will be spawned from based on dificulty
    private List<GameObject> difficultySortList;

    //floats that control delay time between each enemy spawn, time is random between these two values
    private float spawnDelayMin = 0.25f;
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

    //spawnpoints for the enemies, should be a node used for navigation
    [SerializeField]
    private Transform[] spawnPoints;

    private float percentChecker;
    //used to hold an enemy that was spawned to set their parent
    private GameObject enemySpawn;
    //empty parent for the enemies to be in, allows them to be counted easily
    private GameObject enemyParent;        
    //flag for if the current wave is ongoing, used in the wave tracker
    public static bool waveActive;
    public static bool waveSpawning;
    private int enemiesAlive;
    private int totalEnemiesInWave;

    private void Awake()
    { 
        possibleSpawns = new List<GameObject>();
        enemiesToSpawn = new List<GameObject>();
        difficultySortList = new List<GameObject>();
        waveActive = false;
        waveSpawning = false;
    }

    //set it up to allow parameters to be passed through

    public void SetWaveData(WaveScriptableObject waveData, GameObject waveEnemiesParent)
    {
        waveDifficultyThreshold = waveData.waveDifficultyThreshold;
        waveDifficulty = waveData.waveDifficulty;

        balancedBias = waveData.balancedBias;
        fastBias = waveData.fastBias;
        armouredBias = waveData.armouredBias;

        waveMaxValue = waveData.waveValue;
        waveMaxQuantity = waveData.waveQuantity;

        spawnDelayMin = waveData.delayMin;
        spawnDelayMax = waveData.delayMax;

        enemyParent = waveEnemiesParent;
    }
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

        balancedBiasPercent = balancedBias / biasTotal;
        fastBiasPercent = fastBias / biasTotal;
        armouredBiasPercent = armouredBias / biasTotal;
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
        StartCoroutine(FillAndSpawnEnemyList());
    }

    private IEnumerator FillAndSpawnEnemyList()
    {
        waveSpawning = true;
        waveActive = true;

        int currentValue = 0;
        int currentQuantity = 0;

        EnemyDifficultyData difficultyData = null;

        if(difficultySortList != null && difficultySortList.Count > 0)
        {
            difficultySortList.Clear();
        }

        while (currentValue < waveMaxValue && currentQuantity < waveMaxQuantity)
        {
            percentChecker = Random.Range(0f, 1f);

            if (percentChecker <= balancedBiasPercent)
            {
                foreach (GameObject enemy in possibleSpawns)
                {
                    if (enemy.TryGetComponent(out EnemyDifficultyData enemyDifficultyData) && enemyDifficultyData.enemyBias == EnemyDifficultyData.BiasType.Balanced)
                    {
                        difficultySortList.Add(enemyDifficultyData.gameObject);
                        difficultyData = enemyDifficultyData;
                    }
                }
                percentChecker = Random.Range(1f, waveDifficulty);

                difficultySortList.Sort(DifficultySorter);

                currentValue += difficultyData.enemyCost;
                currentQuantity++;

                enemiesToSpawn.Add(difficultySortList[0]);
            }
            else if (percentChecker <= (fastBiasPercent + balancedBiasPercent))
            {
                foreach (GameObject enemy in possibleSpawns)
                {
                    if (enemy.TryGetComponent(out EnemyDifficultyData enemyDifficultyData) && enemyDifficultyData.enemyBias == EnemyDifficultyData.BiasType.Agile)
                    {
                        difficultySortList.Add(enemyDifficultyData.gameObject);
                        difficultyData = enemyDifficultyData;
                    }
                }
                percentChecker = Random.Range(1f, waveDifficulty);

                difficultySortList.Sort(DifficultySorter);

                currentValue += difficultyData.enemyCost;
                currentQuantity++;

                enemiesToSpawn.Add(difficultySortList[0]);
            }
            else if (percentChecker <= (armouredBiasPercent + fastBiasPercent + balancedBiasPercent))
            {
                foreach (GameObject enemy in possibleSpawns)
                {
                    if (enemy.TryGetComponent(out EnemyDifficultyData enemyDifficultyData) && enemyDifficultyData.enemyBias == EnemyDifficultyData.BiasType.Armoured)
                    {
                        difficultySortList.Add(enemyDifficultyData.gameObject);
                        difficultyData = enemyDifficultyData;
                    }
                }
                percentChecker = Random.Range(1f, waveDifficulty);

                difficultySortList.Sort(DifficultySorter);

                currentValue += difficultyData.enemyCost;
                currentQuantity++;

                enemiesToSpawn.Add(difficultySortList[0]);
            }

            yield return null;
        }

        totalEnemiesInWave = enemiesToSpawn.Count;
        enemiesAlive = totalEnemiesInWave;
        UIData.instance.SetWaveBar(1);

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            enemySpawn = Instantiate(enemiesToSpawn[i], spawnPoints[Random.Range(0, spawnPoints.Length)].position, transform.rotation);
            enemySpawn.transform.parent = enemyParent.transform;
            //temporary fix to the navmesh potentialy making enemys stuck, improve enemy AI later
            //Destroy(enemySpawn, 45f);

            yield return new WaitForSecondsRealtime(Random.Range(spawnDelayMin, spawnDelayMax)); 
        }


        waveSpawning = false;
        yield return null;
    }


    private int DifficultySorter(GameObject x, GameObject y)
    {
        int xDifficulty = x.GetComponent<EnemyDifficultyData>().enemyDifficulty;
        int yDifficulty = y.GetComponent<EnemyDifficultyData>().enemyDifficulty;

        xDifficulty = Mathf.Abs(xDifficulty - (int)percentChecker);
        yDifficulty = Mathf.Abs(yDifficulty - (int)percentChecker);

        if(xDifficulty < yDifficulty)
        {
            return -1;
        }
        else if(xDifficulty > yDifficulty)
        {
            return 1;
        }
        return 0;
    }

    public float UpdateWaveProgress(int changeInEnemyCount)
    {
        enemiesAlive += changeInEnemyCount;

        if(enemiesAlive / (float)totalEnemiesInWave == 0)
        {
            waveActive = false;
        }
        return enemiesAlive / (float)totalEnemiesInWave;
    }
}
