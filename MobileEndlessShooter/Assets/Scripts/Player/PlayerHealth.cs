using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private PlayerHealthBar playerHealthBar;
    [SerializeField] private float deathSpinSpeedY = 360f;
    [SerializeField] private float deathSpinSpeedZ = 720f;
    [SerializeField] private float deathAnimationTime = 2f;
    private bool _isDead;

    void Start()
    {
        currentHealth = maxHealth;
        if(playerHealthBar != null)playerHealthBar.SetMaxHealth(maxHealth);
        
    }

    public void TakeDamage(float damage)
    {
        if(_isDead) return;
        Debug.Log("DAMGED " + damage);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        playerHealthBar.SetCurrentHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if(_isDead) return;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        playerHealthBar.SetCurrentHealth(currentHealth);
    }

    public void Die()
    {
        _isDead = true;
        Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(25);
        }
    }
    
   
    
    
}
