using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 targetPoint;
    private Collider colliderEnemy;
    private bool isFollower;
   
    private void Awake()
    {
        player =  GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerMovement>();
        coinSpawnerScript = FindFirstObjectByType<CoinSpawner>();
        animator = GetComponentInChildren<Animator>();
        colliderEnemy = GetComponent<Collider>();
    }

    public void Initialize(EnemyData data) 
    {
        //takes the data from the scriptable object and sets the enemy to it
        this.data = data;
        currentHealth = data.health;
        scoreValue = data.scoreValue;
        isDead = false;
        isFollower = data.isFollower;

        if (!isFollower)
        {
            //sets the pointA and pointB
            pointA = new Vector3(30f, transform.position.y, transform.position.z);
            pointB = new Vector3(-30f, transform.position.y, transform.position.z);

            //randomize starting position to move to
            int randomDirection = Random.Range(0, 2);
            if (randomDirection == 0) targetPoint = pointA;
            else targetPoint = pointB;
        }
        else transform.rotation = Quaternion.LookRotation(player.transform.position);
        colliderEnemy.enabled = true;
    }

    private void Update()
    {
        if (isDead) return;
        
        if (data == null) return;
        
        if (!isFollower)   HandleHorizontalMovement();
        else HandleFollowMovement();
        
        
        if (playerScript != null && (playerScript.transform.position.z) > (transform.position.z + 20f))// check if the player passed the enemy
        {
            RecycleEnemy();
        }
    }

    private void HandleHorizontalMovement()
    {
        // Move ONLY the X axis value towards the target's X value
        float newX = Mathf.MoveTowards(transform.position.x, targetPoint.x, data.movementSpeed * Time.deltaTime); 
    
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
        float targetX = Mathf.MoveTowards(transform.position.x, player.transform.position.x, data.movementSpeed * Time.deltaTime);
        float targetZ = Mathf.MoveTowards(transform.position.z, player.transform.position.z, data.movementSpeed * Time.deltaTime);
        transform.position = new Vector3(targetX, transform.position.y, targetZ);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
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
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !isDead) RecycleEnemy();
    }
}