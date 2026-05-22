using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EndlessShooter/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Visuals")] public GameObject enemyPrefab; 
    public string enemyType; // the enemy type

    [Header("Movement")] public float movementSpeed = 4f;

    [Header("Combat Stats")] public int health = 10;
    public int damageToPlayer = 1;

    [Header("Scoring")] public int scoreValue = 100;
}
