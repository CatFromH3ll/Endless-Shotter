using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private float shieldDuration = 10.0f;
    [SerializeField] private float shieldCooldown = 30.0f;
    private bool canUseShield = true;
    [SerializeField] private Renderer shieldRenderer;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Button shieldButton;

    private void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        shieldRenderer.enabled = false;
        shieldButton.interactable = true;
        
    }

    public void ActivateShield()
    {
        if (canUseShield)
        {
            StartCoroutine(ShieldStart());
        }
        
    }
    private void DeactivateShield()
    {
        AudioManager.instance.ShieldDeactivatedSound();
        shieldRenderer.enabled = false;
        playerHealth.IsShielded = false;
        AudioManager.instance.PlayerEngineMotor();
    }
    

    private IEnumerator ShieldStart()
    {
        AudioManager.instance.ShieldSound();
        canUseShield = false;
        shieldRenderer.enabled = true;
        playerHealth.IsShielded = true;
        shieldButton.interactable = false;

        // Shield stays active for 10 seconds
        yield return new WaitForSeconds(shieldDuration);

        // Shield ends
        DeactivateShield();

        // Cooldown starts after shield ends
        yield return new WaitForSeconds(shieldCooldown);

        // Shield is ready again
        AudioManager.instance.ShieldReadySound();
        canUseShield = true;
        shieldButton.interactable = true;
    }
}
