using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private PlayerHealthBar playerHealthBar;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private UIControler uIControler;
    private bool isDead;
    private bool isShielded;

    void Start()
    {
        currentHealth = maxHealth;
        if(playerHealthBar != null)playerHealthBar.SetMaxHealth(maxHealth);
        playerAnimator = GetComponentInChildren<Animator>();
        
    }
    

    public bool IsDead
    {
        get
        {
            return isDead;
        }
        set
        {
            isDead = value;
        }
    }

    public bool IsShielded
    {
        get
        {
            return isShielded;
        }
        set
        {
            isShielded = value;
        }
    }


    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        } 
        set
        {
            currentHealth = value;
        }
    }

    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }
    
    
    public void SetHealthData(float newMaxHealth)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);

        // Current health cannot be higher than this boat's max health.
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Usually when selecting a boat, you want to start full health.
        currentHealth = maxHealth;
        playerHealthBar.SetMaxHealth(maxHealth);
        playerHealthBar.SetCurrentHealth(currentHealth);
        
    }
    

    public void TakeDamage(float damage)
    {
        if(isDead) return;
        if (isShielded)
        {
            AudioManager.instance.ShieldHitSound();
            return;
        }
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        playerHealthBar.SetCurrentHealth(currentHealth);
        AudioManager.instance.PlayerHit();
        playerAnimator.SetTrigger("Hit");
        

        if (currentHealth <= 0)
        {
            IsDead = true;
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if(IsDead) return;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        playerHealthBar.SetCurrentHealth(currentHealth);
    }

    public void Die()
    {
        AudioManager.instance.PlayerDeath();
        playerAnimator.SetTrigger("Death");
        uIControler.Death();
        Debug.Log("DEAD");
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject enemy = collision.gameObject;
            EnemyController controller = enemy.GetComponent<EnemyController>();
            TakeDamage(controller.damageOnImpact);
        }

        if (collision.gameObject.CompareTag("Hazard"))
        {
            GameObject hazard = collision.gameObject;
            Hazards hazardScript = hazard.GetComponent<Hazards>();
            TakeDamage(hazardScript.damage);
        }
    }
    
   
    
    
}
