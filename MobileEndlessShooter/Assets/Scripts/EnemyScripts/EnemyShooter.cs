using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    
    // We use a different pool hash for enemy bullets to keep them separate from player bullets
    private const string projectileHash = "EnemyProjectile"; 
    private float fireTimer;

    private void Update()
    {
        fireTimer += Time.deltaTime;
    }

    public void TryShoot()
    {
        Debug.Log("tryShooting");
        if (fireTimer >= currentWeapon.fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    private void Shoot()
    {
        Debug.Log("shooting");
        GameObject projectile = GenericObjectPooler.Instance.GetFromPool(
            projectileHash,
            projectilePrefab,
            shootingPoint.position,
            shootingPoint.rotation
        );

        if (projectile.TryGetComponent<Projectile>(out var projectileScript))
        {
            projectileScript.SetDamage(currentWeapon.projectileDamage);
            projectileScript.LaunchProjectile(shootingPoint.forward, currentWeapon.projectileSpeed, true);
        }
    }
}
