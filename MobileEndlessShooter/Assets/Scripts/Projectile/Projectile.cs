using UnityEngine;

public class Projectile : MonoBehaviour
{
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
    }

    private void OnEnable()
    {
        
        projectileTimer = 0f;
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
        }

        if (collision.gameObject.CompareTag("Player") && isEnemy)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
           playerHealth.TakeDamage(damage);
        }
        ReturnToProjectilePool();
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
