using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rbProjectile;
    [SerializeField]private float damage;
    [SerializeField]private float projectileTimer;
    [SerializeField]private float time = 3f;
    public Projectile projectilePrefab { get; private set; }
    private Vector3 spinSpeed = new (0f, 800f, 0f);
    [SerializeField]private Transform rocketModel;
    

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
        if (collision.gameObject.CompareTag("Player")) return;
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            enemyController.TakeDamage(damage);
        }
        ReturnToProjectilePool();
    }
    
    
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void LaunchProjectile(Vector3 direction, float speed)
    {
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
