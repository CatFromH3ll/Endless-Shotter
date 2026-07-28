using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float executionLookAheadTime = 0.2f;
    [SerializeField] private int minExecutionNum = 0;
    [SerializeField] private int maxExecutionNum = 100;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private ParticleSystem smokeParticles;
    private TimelineManager timelineManager;
    private bool executionTriggered;
    [SerializeField] private float minimumExecutionDistance = 1.5f;
    
    
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
        smokeParticles = GetComponentInChildren<ParticleSystem>();
        timelineManager = FindObjectOfType<TimelineManager>();
    }

    private void OnEnable()
    {
        projectileTimer = 0f;
        executionTriggered = false;
        smokeParticles.Play();
    }

    private void FixedUpdate()
    {
        // Count the actual distance travelled by the projectile.
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
    

    public void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.CompareTag("Floor") || (collision.gameObject.CompareTag("Player") && !isEnemy)) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            ContactPoint contact = collision.contacts[0];

            SpawnExplosion(contact.point, contact.normal);
            enemyController.TakeDamage(damage);
            if (executionTriggered && timelineManager.ExecutionPlaying)
            {
                timelineManager.EndExecution();
            }
            ReturnToProjectilePool();
            
        }

        if (collision.gameObject.CompareTag("Player") && isEnemy)
        { 
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
           playerHealth.TakeDamage(damage);
           ReturnToProjectilePool();
        }

        if (collision.gameObject.CompareTag("Hazard"))
        {
            ReturnToProjectilePool();
            ContactPoint contact = collision.contacts[0];

            SpawnExplosion(contact.point, contact.normal);
        }
        
        smokeParticles.Stop();
    }
    
    private void SpawnExplosion(Vector3 position, Vector3 surfaceNormal)
    {
        Quaternion rotation = Quaternion.LookRotation(surfaceNormal);

        GameObject explosion = Instantiate(
            explosionPrefab,
            position,
            rotation
        );

        // Remove the explosion after its particles finish.
        Destroy(explosion, 5f);
    }

    private void CheckForLethalImpact()
    {
        
        if (isEnemy)
            return;
        
        if (timelineManager == null)
            return;
        
        if(timelineManager.ExecutionPlaying)
            return;
        
        if (projectileTimer <= minimumExecutionDistance)
            return;
        
        
        Vector3 velocity = rbProjectile.linearVelocity;

        if (velocity.sqrMagnitude <= 0.001f)
            return;

        Vector3 direction = velocity.normalized;

        float checkDistance =
            velocity.magnitude * timelineManager.SlowMotionScale * executionLookAheadTime;
        Debug.Log("checkDistance " + velocity.magnitude * timelineManager.SlowMotionScale * executionLookAheadTime);

        bool aboutToHitSomething = rbProjectile.SweepTest(
            direction,
            out RaycastHit hit,
            checkDistance,
            QueryTriggerInteraction.Ignore
        );
        Debug.DrawRay(rbProjectile.position, direction * checkDistance, Color.red);
        
        if (!aboutToHitSomething)
            return;
        
        

        EnemyController enemy =
            hit.collider.GetComponentInParent<EnemyController>();

        if (enemy == null)
            return;
        if(enemy.waveType == WaveType.Shooting)return;
        int random =  Random.Range(minExecutionNum, maxExecutionNum);
        if (random >= 10)
        {
            return;
        }

        if (enemy.WillDieFromDamage(damage))
        {
            timelineManager.StartExecution(
                gameObject,
                enemy.transform
            );
            
        }
            
        

        
        
        
        
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
