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
    
    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;
    

    public void TakeDamage(float damage)
    {
        if(isDead) return;
        if (isShielded)
        {
            AudioManager.instance.ShieldHitSound();
            return;
        }
        Debug.Log("DAMGED " + damage);
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
