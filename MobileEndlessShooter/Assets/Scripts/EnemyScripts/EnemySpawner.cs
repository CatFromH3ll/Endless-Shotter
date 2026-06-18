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
    private int currentActiveEnemies = 0;
    private int stage;
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
        if (currentActiveEnemies <= 0)
        {
            currentDifficultyIndex++;
        }

        if (currentDifficultyIndex >= difficultyTimeline.Length && currentActiveEnemies > 0)
        {
            currentDifficultyIndex = 0;
            stage++;
            Console.WriteLine("stage " + stage);
        }
        
        //levelTimer += Time.deltaTime; 
       // // check if enaght time passed to move into a next faze 
       // if (levelTimer >= CurrentDifficulty.durationInSeconds)
       // {
       //     if (currentDifficultyIndex < difficultyTimeline.Length - 1)
       //     {
       //         currentDifficultyIndex++;
       //         levelTimer = 0f;
       //         Debug.Log($"Difficulty scaled up automatically to: {CurrentDifficulty.difficultyName}");
       //         
       //     }
       // }
//
       // if (levelTimer >= CurrentDifficulty.durationInSeconds &&
       //     currentDifficultyIndex == difficultyTimeline.Length - 1)
       // {
       //     uiControler.Finish();
       // }
    }

    private void HandleSpawningIntervals()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= CurrentDifficulty.spawnInterval)
        {
            spawnTimer = 0f;

            // only spawn enemy's if there are less then the maximum amount of enemy's
            //for (int i = 0; i < CurrentDifficulty.EnemiesToSpawn ; i++)
            {
                SpawnRandomEnemy();
            }
        }
    }

    private void SpawnRandomEnemy()
    {
        if (CurrentDifficulty.allowedEnemies.Length == 0) return;

        
        EnemyData selectedEnemyData = ChooseEnemyByWeight();
        if (selectedEnemyData == null || selectedEnemyData.enemyPrefab == null) return; 

        //randomize the x value of the position that the enemy will spawn in
        float randomX = Random.Range(-2.5f, 2.5f);
        // set the spawn point to the randomized x and the z is the position of the player + distanse to spawn from the player
        float yOffset = 3.5f;
        Vector3 spawnPosition = new Vector3(randomX, yOffset, player.transform.position.z + distanceFromPlayer);

        // requese the poold object from the large pool
        GameObject spawnedEnemy = GenericObjectPooler.Instance.GetFromPool(
            selectedEnemyData.enemyType, 
            selectedEnemyData.enemyPrefab, 
            spawnPosition, 
            selectedEnemyData.enemyPrefab.transform.rotation
        );

        //  set the enemy script data to the disierd enemy settings
        if (spawnedEnemy.TryGetComponent<EnemyController>(out var enemyScript))
        {
            enemyScript.Initialize(selectedEnemyData);
            currentActiveEnemies++;
        }
    }

    private EnemyData ChooseEnemyByWeight()
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