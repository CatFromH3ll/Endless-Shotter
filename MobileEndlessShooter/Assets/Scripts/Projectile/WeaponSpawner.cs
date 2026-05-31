using System;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private GameObject projectilePrefab;
    private const string projectileHash = "Projectile";
    PlayerHealth playerHealth;
    private float fireTimer;

    public void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>().gameObject.GetComponent<PlayerHealth>();
    }

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
    
    public void Shoot()
    {
        if(playerHealth.IsDead) return;
        GameObject projectile = GenericObjectPooler.Instance.GetFromPool(projectileHash, projectilePrefab, spawnPoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetDamage(currentWeapon.projectileDamage);
        projectileScript.LaunchProjectile(spawnPoint.forward, currentWeapon.projectileSpeed);
    }

    
    
}
