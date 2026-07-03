using System;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    
    [SerializeField] private WeaponData currentWeapon;
    PlayerHealth playerHealth;
    private float fireTimer;

    public void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>().gameObject.GetComponent<PlayerHealth>();
        fireTimer = currentWeapon.fireRate;
    }

    public void Update()
    {
        fireTimer += Time.deltaTime;
    }
    
    public void SetWeaponData(WeaponData newWeaponData)
    {
        currentWeapon = newWeaponData;
    }

    public void ShootTimer()
    {
        
        if (fireTimer < currentWeapon.fireRate)
        {
            return;
        }
        if(playerHealth.IsDead) return;
        Shoot();
        fireTimer = 0;
    }
    
    public void Shoot() 
    {
        
        GameObject projectile = GenericObjectPooler.Instance.GetFromPool(currentWeapon.poolKey, currentWeapon.projectilePrefab, spawnPoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetDamage(currentWeapon.projectileDamage);
        projectileScript.LaunchProjectile(spawnPoint.forward, currentWeapon.projectileSpeed);
        AudioManager.instance.PlayerBasicShotSound();
    }

    
    
}
