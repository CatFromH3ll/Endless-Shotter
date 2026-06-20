using UnityEngine;

public class Heal : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerHealth playerHealth;
    private const string healHash = "Heal";
    
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (player != null && (player.transform.position.z) > (transform.position.z + 20f))
        {
            RecycleHeal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out playerHealth))
        {
            return;
        }

        if (playerHealth.CurrentHealth >= playerHealth.MaxHealth)
        {
            return;
        }
        playerHealth.Heal(25);
        AudioManager.instance.RepairSound();
        RecycleHeal();
    }

    public void RecycleHeal()
    {
        gameObject.SetActive(false);
    }
    
    
    
    
    
    
}


