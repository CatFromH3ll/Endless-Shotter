using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public Projectile projectilePrefab;
    
    public float projectileSpeed = 20f;
    public float projectileDamage = 20f;
    public float fireRate = 0.5f;
}
