using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    [SerializeField] private int projectilePoolSize = 10;
    private Dictionary<Projectile, Queue<Projectile>> projectilePool =  new Dictionary<Projectile, Queue<Projectile>>();
    [SerializeField]private Transform spawnPoint;
    void Start()
    {
        
    }
    public Projectile GetProjectile(Projectile projectilePrefab)
    {
        if (!projectilePool.ContainsKey(projectilePrefab))
        {
            CreatePool(projectilePrefab);
        }

        Queue<Projectile> pool = projectilePool[projectilePrefab];

        if (pool.Count == 0)
        {
            CreateProjectile(projectilePrefab);
        }
        
        Projectile projectile = pool.Dequeue();
        projectile.gameObject.SetActive(true);
        
        return projectile;
    }

    private void CreatePool(Projectile projectilePrefab)
    {
        projectilePool.Add(projectilePrefab, new Queue<Projectile>());

        for (int i = 0; i < projectilePoolSize; i++)
        {
            CreateProjectile(projectilePrefab);
        }
    }

    private Projectile CreateProjectile(Projectile projectilePrefab)
    {
        Projectile projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        
        projectile.SetProjectilePooler(this);
        projectile.SetProjectilePrefab(projectilePrefab);
        projectile.gameObject.SetActive(false);
        projectilePool[projectilePrefab].Enqueue(projectile);
        
        return projectile;
    }

    public void ReturnProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        Projectile OriginalProjectile = projectile.projectilePrefab;

        if (!projectilePool.ContainsKey(projectile))
        {
            projectilePool.Add(projectile, new Queue<Projectile>());
        }
        projectilePool[OriginalProjectile].Enqueue(projectile);
        
    }
    
}
