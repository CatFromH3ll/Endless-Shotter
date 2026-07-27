using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float executionLookAheadTime = 0.2f;
    [SerializeField] private int minExecutionNum = 0;
    [SerializeField] private int maxExecutionNum = 50;
    private TimelineManager timelineManager;
    private bool executionTriggered;
    [SerializeField] private float minimumExecutionDistance = 1.5f;
    //private float distanceTravelled;
    
    
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
        //distanceTravelled = 0f;
    }

    private void FixedUpdate()
    {
        // Count the actual distance travelled by the projectile.
        //distanceTravelled += Time.fixedDeltaTime;
        Vector3 velocity = rbProjectile.linearVelocity;
        Debug.Log("slowmoscale " + timelineManager.SlowMotionScale);
        Debug.Log("velocity magni" + velocity.magnitude);
        Debug.Log("execution look ahead tme " + executionLookAheadTime);
        Debug.Log("checkDistance " + velocity.magnitude * timelineManager.SlowMotionScale * executionLookAheadTime);
        
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
        }
        
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
        
        /*if (executionTriggered)
            return;*/

        
        
        
        
        

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

        if (enemy.WillDieFromDamage(damage))
        {
            timelineManager.StartExecution(
                gameObject,
                enemy.transform
            );
            /*if (started)
            {
                executionTriggered = true;
            }*/
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
