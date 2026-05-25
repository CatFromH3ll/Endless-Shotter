using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ProjectilePooler pooler;
    [SerializeField] private Rigidbody rbProjectile;
    [SerializeField]private float damage;
    [SerializeField]private float projectileTimer;
    [SerializeField]private float time = 3f;
    public Projectile projectilePrefab { get; private set; }
    

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
        if (projectileTimer >= time)
        {
            ReturnToProjectilePool();
            projectileTimer = 0f;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") return;
        ReturnToProjectilePool();
    }

    public void SetProjectilePooler(ProjectilePooler newPooler)
    {
        pooler = newPooler;
    }

    public void SetProjectilePrefab(Projectile newProjectile)
    {
        projectilePrefab = newProjectile;
    }
    
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void LaunchProjectile(Vector3 direction, float speed)
    {
        rbProjectile.linearVelocity = direction.normalized * speed;
        rbProjectile.angularVelocity = Vector3.zero;
        //rbProjectile.AddForce(direction.normalized * speed, ForceMode.Impulse);
    }

    private void ReturnToProjectilePool()
    {
        rbProjectile.linearVelocity = Vector3.zero;
        rbProjectile.angularVelocity = Vector3.zero;
        pooler.ReturnProjectile(this);
    }
}
