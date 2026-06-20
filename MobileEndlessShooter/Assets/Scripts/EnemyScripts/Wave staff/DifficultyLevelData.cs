using UnityEngine;

public enum WaveType
{
    Patrol,
    Kamikaze,
    Shooting
}
[System.Serializable] // makes it so the inspector can show this struct 

public struct EnemySpawnWeight
{
    public EnemyData enemyData;       //referring to the enemy scriptable object
    [Range(0f, 1f)] public float weight; //chance to spawn specific things - the higher the number the higher the odds
}

[CreateAssetMenu(fileName = "NewDifficultyLevel", menuName = "EndlessShooter/Difficulty Level")]
public class DifficultyLevelData : ScriptableObject
{
    [Header("Enemy Variety Pool")]
    public EnemySpawnWeight[] allowedEnemies; //the list of enemy types in the wave
    
    [Header("Level Configuration")] //difficult configurations
    
    public WaveType chosenWaveType;
    public bool bossWave;
    public bool bigVersion;
    public float spawnInterval = 2f; //how much time passes between enemy spawns
    public int EnemiesToSpawn = 10;
    public int amountToSpawnEachSpawning;



}
 