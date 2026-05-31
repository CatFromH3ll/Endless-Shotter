using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private EnemyData data;
    private float currentHealth;
    private bool isDead = false;
    private const string DeadHash = "Dead";
    private PlayerMovement player;
    private Animator animator;
    private CoinSpawner coinSpawnerScript;
    private int scoreValue;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 targetPoint;
    private Collider colliderEnemy;
    
    private Vector3 startingLocalPosition;
    private Quaternion startingLocalRotation;
   
    private void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        coinSpawnerScript = FindFirstObjectByType<CoinSpawner>();
        animator = GetComponentInChildren<Animator>();
        colliderEnemy = GetComponent<Collider>();
        
        startingLocalPosition = animator.transform.localPosition;
        startingLocalRotation = animator.transform.localRotation;
    }

    public void Initialize(EnemyData data) 
    {
        //takes the data from the scriptable object and sets the enemy to it
        this.data = data;
        currentHealth = data.health;
        scoreValue = data.scoreValue;
        isDead = false;

        //sets the pointA and pointB
        pointA = new Vector3(30f, transform.position.y, transform.position.z);   
        pointB = new Vector3(-30f, transform.position.y, transform.position.z);   
        
        //randomize starting position to move to
        int randomDirection = Random.Range(0, 2);
        if  (randomDirection == 0) targetPoint = pointA;
        else targetPoint = pointB;
       
       colliderEnemy.enabled = true;
    }

    private void Update()
    {
        if (isDead) return;
        
        if (data == null) return;
        
        HandleHorizontalMovement();

       
        if (player != null && (player.transform.position.z) > (transform.position.z + 20f))// check if the player passed the enemy
        {
            RecycleEnemy();
        }
    }

    private void HandleHorizontalMovement()
    {
        //moves the enemy to target (pointA/pointB)
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, data.movementSpeed * Time.deltaTime); 
        

       
        if (Mathf.Approximately(transform.position.x, targetPoint.x)) // will check if the enemy (approximately) arrive at the point destination
        {
            targetPoint = (targetPoint == pointB) ? pointA : pointB; // will switch the point to move to
        }
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
        Debug.Log("recycled");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !isDead) RecycleEnemy();
    }
}