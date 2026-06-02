using UnityEngine;

[System.Serializable] // makes it so the inspector can show this struct 
public struct EnemySpawnWeight
{
    public EnemyData enemyData;       //referring to the enemy scriptable object
    [Range(0f, 1f)] public float weight; //chance to spawn specific things - the higher the number the higher the odds
}

[CreateAssetMenu(fileName = "NewDifficultyLevel", menuName = "EndlessShooter/Difficulty Level")]
public class DifficultyLevelData : ScriptableObject
{
    [Header("Level Configuration")] //difficult configurations
    public string difficultyName = "Easy";
    public float durationInSeconds = 30f; //how long dos the wave last
    public float spawnInterval = 2f; //how much time passes between enemy spawns
    public int maxActiveEnemies = 10; //check that there are no more than X enemy's active

    [Header("Enemy Variety Pool")]
    public EnemySpawnWeight[] allowedEnemies; //the list of enemy types in the wave
}