using System;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private ProjectilePooler projectilePool;
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WeaponData currentWeapon;
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

    private void Shoot()
    {
        if(playerHealth.IsDead) return;
        Projectile projectile = projectilePool.GetProjectile(currentWeapon.projectilePrefab);
        projectile.ReturnToProjectilePool();
        projectile.transform.position = spawnPoint.position;
        projectile.transform.rotation = spawnPoint.rotation;
        projectile.gameObject.SetActive(true);
        projectile.SetDamage(currentWeapon.projectileDamage);
        projectile.LaunchProjectile(spawnPoint.forward, currentWeapon.projectileSpeed);
    }
}
