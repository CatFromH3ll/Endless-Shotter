using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private ProjectilePooler projectilePool;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WeaponData currentWeapon;
    
    private float fireTimer;


    public void Update()
    {
        fireTimer += Time.deltaTime;
    }

    public void ShootTimer()
    {
        if (fireTimer < currentWeapon.fireRate)
        {
            Debug.Log("cooldown");
            return;
        }
        Debug.Log("ready to fire");
        Shoot();
        fireTimer = 0;
    }

    private void Shoot()
    {
        Debug.Log("FIRE");
        Debug.Log("projectilePooler = " + projectilePool);
        Debug.Log("currentWeapon = " + currentWeapon);
        Debug.Log("spawnPoint = " + spawnPoint);
        Projectile projectile = projectilePool.GetProjectile(currentWeapon.projectilePrefab);
        Debug.Log("Got projectile: " + projectile);
        projectile.transform.position = spawnPoint.position;
        projectile.transform.rotation = spawnPoint.rotation;
        projectile.SetDamage(currentWeapon.projectileDamage);
        projectile.LaunchProjectile(spawnPoint.forward, currentWeapon.projectileSpeed);
    }
}
