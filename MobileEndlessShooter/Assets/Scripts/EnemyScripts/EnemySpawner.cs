using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement player;
    
    [Header("Progression Timeline Data")]
    [SerializeField] private DifficultyLevelData[] difficultyTimeline;

    private int currentDifficultyIndex = 0;
   // private float levelTimer = 0f;
    private float spawnTimer = 0f;
    public int currentActiveEnemies = 0;
    private int spawndEnemysThisWave;
    private int stage;
    private bool finishdSpawning = false;
    [SerializeField] private float distanceFromPlayer = 60f;
    [SerializeField] private UIControler uiControler;
    
    //a short way (using the lambda) to set the current difficulty to the desired difficulty by time
    private DifficultyLevelData CurrentDifficulty => difficultyTimeline[currentDifficultyIndex];

    private void Update()
    {
        if (player == null || difficultyTimeline.Length == 0) return;

        HandleProgressionTimeline();
        HandleSpawningIntervals();
        
    }

    private void HandleProgressionTimeline()
    {
        if (currentActiveEnemies <= 0 && spawndEnemysThisWave >= CurrentDifficulty.EnemiesToSpawn)
        {
            spawndEnemysThisWave = 0;
            // Only increment if we haven't reached the end yet
            if (currentDifficultyIndex < difficultyTimeline.Length - 1)
            {
                currentDifficultyIndex++;
            }
            else
            {
                currentDifficultyIndex = 0;
                stage++;
                Debug.Log(stage);
            }
        }
    }

    private void HandleSpawningIntervals()
    {
        spawnTimer += Time.deltaTime;
            
        if (spawnTimer >= CurrentDifficulty.spawnInterval &&         //checks if the time between spawning passed
            (CurrentDifficulty.EnemiesToSpawn >= spawndEnemysThisWave))//and if it didn't finished spawning the enemy's
        { 
            Debug.Log(CurrentDifficulty.EnemiesToSpawn);
            for (int i = 0; i < CurrentDifficulty.amountToSpawnEachSpawning ; i++) 
            { 
                spawnTimer = 0f;
                SpawnRandomEnemy(); 
            } 
        }
        
    }

    private void SpawnRandomEnemy()
    {
        if (CurrentDifficulty.allowedEnemies.Length == 0) return;

        
        EnemyData selectedEnemyData = ChooseEnemyByWeight();
        if (selectedEnemyData == null || selectedEnemyData.enemyPrefab == null) return;

        float leftSpawnMargin = 0.4f;
        float rightSpawnMargin = 0.6f;

        float targetZ = player.transform.position.z + distanceFromPlayer;

        float distanceToCamera = targetZ - Camera.main.transform.position.z;

        Vector3 leftSpawnBounds = Camera.main.ViewportToWorldPoint(new Vector3(leftSpawnMargin, 0.5f, distanceToCamera));
        Vector3 rightSpawnBounds = Camera.main.ViewportToWorldPoint(new Vector3(rightSpawnMargin, 0.5f, distanceToCamera));

        float randomX = Random.Range(leftSpawnBounds.x, rightSpawnBounds.x);

        float yOffset = 3.5f;
        Vector3 spawnPosition = new Vector3(randomX, yOffset, targetZ);
        
        GameObject spawnedEnemy = GenericObjectPooler.Instance.GetFromPool(
            selectedEnemyData.enemyType, 
            selectedEnemyData.enemyPrefab, 
            spawnPosition, 
            selectedEnemyData.enemyPrefab.transform.rotation
        );

        if (spawnedEnemy.TryGetComponent<EnemyController>(out var enemyScript))
        {
            enemyScript.Initialize(selectedEnemyData,CurrentDifficulty);
            currentActiveEnemies++;
            spawndEnemysThisWave++;
        }
    }

    private EnemyData ChooseEnemyByWeight()// choos a random enemy from the wave pool
    {
        float totalWeight = 0f;
        foreach (var enemyWeight in CurrentDifficulty.allowedEnemies) totalWeight += enemyWeight.weight; // adds the weight value of each enemy type in the wave.

        float randomVal = Random.Range(0f, totalWeight); // generate a random number from 0 to the total weight
        float weightSum = 0f; 
        
        /* for each enemy type add it weight and check if the total weight is above or equal to the random value
         that was generated if it is return the enemy */
        foreach (var enemyWeight in CurrentDifficulty.allowedEnemies) 
        {
            weightSum += enemyWeight.weight;
            if (randomVal <= weightSum)
            {
                return enemyWeight.enemyData;
            }
        }
        return CurrentDifficulty.allowedEnemies[0].enemyData; // return the first enemy in the array in case of a floating point error
    }
}