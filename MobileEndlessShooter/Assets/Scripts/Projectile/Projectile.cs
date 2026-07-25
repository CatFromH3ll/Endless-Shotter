using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float executionLookAheadTime = 0.2f;
    [SerializeField] private int minExecutionNum = 0;
    [SerializeField] private int maxExecutionNum = 50;
    private TimelineManager timelineManager;
    private bool executionTriggered;
    
    
    [SerializeField] private Rigidbody rbProjectile;
    [SerializeField]private float damage;
    [SerializeField]private float projectileTimer;
    [SerializeField]private float time = 3f;
    private Vector3 spinSpeed = new (0f, 800f, 0f);
    [SerializeField]private Transform rocketModel;
    private bool isEnemy;
    

    void Awake()
    {
        rbProjectile = GetComponent<Rigidbody>();
        
        timelineManager = FindObjectOfType<TimelineManager>();
    }

    private void OnEnable()
    {
        projectileTimer = 0f;
        executionTriggered = false;
    }

    private void FixedUpdate()
    {
        CheckForLethalImpact();
    }

    public void Update()
    {
        projectileTimer += Time.deltaTime;
        if (rocketModel != null)
        {
            rocketModel.Rotate(spinSpeed * Time.deltaTime);
        }
        if (projectileTimer >= time)
        {
            ReturnToProjectilePool();
            projectileTimer = 0f;
        }
    }
    void OnTriggerEnter(Collider other) // (or OnCollisionEnter, depending on your setup)
    {
        if (other.CompareTag("Enemy"))
        {
            // Find your manager script and force the execution to stop instantly
            TimelineManager timelineManager = FindObjectOfType<TimelineManager>(); // Replace 'TimelineManager' with the actual name of your script!
            if (timelineManager != null)
            {
                timelineManager.ForceStopExecution();
            }

            // Now it is safe to destroy the bullet!
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.CompareTag("Floor") || (collision.gameObject.CompareTag("Player") && !isEnemy)) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            
            enemyController.TakeDamage(damage);
        }

        if (collision.gameObject.CompareTag("Player") && isEnemy)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
           playerHealth.TakeDamage(damage);
        }
        ReturnToProjectilePool();
    }

    private void CheckForLethalImpact()
    {
        if (isEnemy)
            return;

        if (executionTriggered)
            return;

        if (timelineManager == null)
            return;

        Vector3 velocity = rbProjectile.linearVelocity;

        if (velocity.sqrMagnitude <= 0.001f)
            return;

        Vector3 direction = velocity.normalized;

        float checkDistance =
            velocity.magnitude * executionLookAheadTime;

        bool aboutToHitSomething = rbProjectile.SweepTest(
            direction,
            out RaycastHit hit,
            checkDistance,
            QueryTriggerInteraction.Ignore
        );

        if (!aboutToHitSomething)
            return;

        EnemyController enemy =
            hit.collider.GetComponentInParent<EnemyController>();

        if (enemy == null)
            return;

        if (!enemy.WillDieFromDamage(damage))
            return;
        bool started = timelineManager.StartExecution(
            gameObject,
            enemy.transform
        );

        if (started)
        {
            executionTriggered = true;
        }
        /*int random =  Random.Range(minExecutionNum, maxExecutionNum);
        if (random <= 5)
        {
            
        }*/
        
        
    }
    
    
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void LaunchProjectile(Vector3 direction, float speed, bool isEnemy = false)
    {
        this.isEnemy = isEnemy;
        rbProjectile.linearVelocity = direction.normalized * speed;
        
        rbProjectile.angularVelocity = Vector3.zero;
    }

    public void ReturnToProjectilePool()
    {
        rbProjectile.linearVelocity = Vector3.zero;
        rbProjectile.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
