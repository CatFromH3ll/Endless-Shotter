using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private PlayerHealthBar playerHealthBar;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private UIControler uIControler;
    private bool isDead;

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
        private  set
        {
            isDead = value;
        }
    }

    public void TakeDamage(float damage)
    {
        if(isDead) return;
        Debug.Log("DAMGED " + damage);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        playerHealthBar.SetCurrentHealth(currentHealth);
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
        playerAnimator.SetTrigger("Death");
        uIControler.Death();
        Debug.Log("DEAD");
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(25);
        }
    }
    
   
    
    
}
