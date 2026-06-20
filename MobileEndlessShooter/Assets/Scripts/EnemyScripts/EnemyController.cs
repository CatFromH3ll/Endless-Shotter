using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class EnemyController : MonoBehaviour
{
    private EnemyData data;
    private float currentHealth;
    private bool isDead = false;
    private const string DeadHash = "Dead";
    private PlayerMovement playerScript;
    private GameObject player;
    private Animator animator;
    private CoinSpawner coinSpawnerScript;
    private int scoreValue;
    private EnemySpawner enemySpawner;
    private float offset = 3.0f;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 targetPoint;
    private Collider colliderEnemy;
    public DifficultyLevelData currentDifficulty;
    private WaveType waveType;
    
    private float screenWidth = Screen.width;
    private float currentSpeed;
   
    private void Awake()
    {
        player =  GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerMovement>();
        coinSpawnerScript = FindFirstObjectByType<CoinSpawner>();
        animator = GetComponentInChildren<Animator>();
        colliderEnemy = GetComponent<Collider>();
        enemySpawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemySpawner>();
        
    }

    public void Initialize(EnemyData data, DifficultyLevelData currentDifficulty) 
    {
        //takes the data from the scriptable object and sets the enemy to it
        this.data = data;
        this.currentDifficulty = currentDifficulty;
        currentHealth = data.health;
        scoreValue = data.scoreValue;
        isDead = false;
        waveType = currentDifficulty.chosenWaveType;
        currentSpeed = data.movementSpeed;

        switch (waveType)
        {
            case WaveType.Kamikaze:
            {
                transform.rotation = Quaternion.LookRotation(player.transform.position);
                currentSpeed *= 3;
                break;
            }
            case WaveType.Patrol:
            {
                //sets the pointA and pointB
            
            
                float leftMargin = 0.4f;
                float rightMargin = 0.6f;
            
                Vector3 leftPoint = Camera.main.ViewportToWorldPoint
                    (new Vector3(leftMargin,0.5f, transform.position.z - Camera.main.transform.position.z));
            
                Vector3 rightPoint = Camera.main.ViewportToWorldPoint
                    (new Vector3(rightMargin ,0.5f , transform.position.z - Camera.main.transform.position.z));

                pointA = new Vector3(rightPoint.x, transform.position.y, transform.position.z);
                pointB = new Vector3(leftPoint.x, transform.position.y, transform.position.z);
            
                //randomize starting position to move to
                targetPoint = (Random.Range(0, 2) == 0) ? pointA : pointB;

                break;
            }
            case WaveType.Shooting:
            {
                transform.rotation = Quaternion.LookRotation(player.transform.position);
                break;
            }
        }

        if (currentDifficulty.bigVersion)
        {
            float scaleMultyplayer = 1.5f;
            currentHealth *= scaleMultyplayer;
            transform.localScale = new Vector3(scaleMultyplayer, scaleMultyplayer, scaleMultyplayer);
        }
        colliderEnemy.enabled = true;
    }

    private void Update()
    {
        if (isDead) return;
        if (data == null) return;
        
        switch (waveType)
        {
            case WaveType.Kamikaze:
            {
                HandleFollowMovement();
                break;
            }
            case WaveType.Patrol:
            {
                HandleHorizontalMovement();
                break;
            }
            case WaveType.Shooting:
            {
                HandleShootingBehavior();
                break;
            }
        }
        
        if (playerScript != null && (playerScript.transform.position.z) > (transform.position.z + 20f))// check if the player passed the enemy
        {
            RecycleEnemy();
        }
    }

    private void HandleShootingBehavior()
    {
        float offsetToPLayer = 100f;
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offsetToPLayer);
    }

    private void HandleHorizontalMovement()
    {
        // Move ONLY the X axis value towards the target's X value
        float newX = Mathf.MoveTowards(transform.position.x, targetPoint.x, currentSpeed * Time.deltaTime); 
    
        // Apply the new X, but keep the current Y and Z exactly as they are right now!
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Check if the enemy arrived at the destination X
        if (Mathf.Approximately(transform.position.x, targetPoint.x)) 
        {
            targetPoint = (targetPoint == pointB) ? pointA : pointB; // switch points
        }
    }

    private void HandleFollowMovement()
    {
        float targetX = Mathf.MoveTowards(transform.position.x, player.transform.position.x, currentSpeed * Time.deltaTime);
        float targetZ = Mathf.MoveTowards(transform.position.z, player.transform.position.z  - offset, currentSpeed * Time.deltaTime);
        transform.position = new Vector3(targetX, transform.position.y, targetZ);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        AudioManager.instance.PlayerProjectileHitSound();
        if (currentHealth <= 0) Die();
        Debug.Log(currentHealth);
    }

    private void Die() 
    {
        isDead = true;
       animator.SetTrigger(DeadHash);
       coinSpawnerScript.DropCoins(scoreValue, transform.position);
       colliderEnemy.enabled = false;
       Debug.Log("dead");
    }

    public void RecycleEnemy()
    {
        isDead = true;
        gameObject.SetActive(false);
        enemySpawner.currentActiveEnemies--;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !isDead) RecycleEnemy();
    }
}