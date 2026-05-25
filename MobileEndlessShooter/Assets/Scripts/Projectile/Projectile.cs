using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ProjectilePooler pooler;
    [SerializeField] private Rigidbody rbProjectile;
    [SerializeField]private float damage;
    [SerializeField]private float projectileTimer;
    [SerializeField]private float time = 3f;
    public Projectile projectilePrefab { get; private set; }
    Vector3 projectileDirection = Vector3.zero;

    void Awake()
    {
        rbProjectile = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Debug.Log("Projectile enabled: " + gameObject.name);
        projectileTimer += Time.deltaTime;
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
        Debug.Log("Projectile hit: " + collision.gameObject.name);
        ReturnToProjectilePool();
    }

    public void SetProjectilePooler(ProjectilePooler newPooler)
    {
        pooler = newPooler;
    }

    public void SetProjectilePreab(Projectile newProjectile)
    {
        projectilePrefab = newProjectile;
    }
    
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void LaunchProjectile(Vector3 direction, float speed)
    {
        projectileDirection = transform.forward * speed * Time.deltaTime;
        rbProjectile.AddForce(direction * speed, ForceMode.Impulse);
    }

    public void ReturnToProjectilePool()
    {
        Debug.Log("Projectile returned to pool: " + gameObject.name);
        rbProjectile.linearVelocity = Vector3.zero;
        rbProjectile.angularVelocity = Vector3.zero;
        pooler.ReturnProjectile(this);
    }
}
