using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class PlayerAbilities : MonoBehaviour
{
    PlayerData playerData;
    [SerializeField] private float shieldDuration = 15.0f;
    [SerializeField] private float currentShieldDuration;
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
        Debug.Log(shieldDuration);

    }

    public float GetShieldDuration
    {
        get
        {
            return shieldDuration;
        }
        set
        {
            shieldDuration = value;
        }
    }
    

    private void ActivateShield()
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

    public void SetShieldDuration(float newDuration)
    {
        shieldDuration = Mathf.Max(0, newDuration);
        currentShieldDuration = Mathf.Clamp(currentShieldDuration, 0f, shieldDuration);
    }


    private IEnumerator ShieldStart()
    {
        AudioManager.instance.ShieldSound();
        canUseShield = false;
        shieldRenderer.enabled = true;
        playerHealth.IsShielded = true;
        shieldButton.interactable = false;

        // Shield stays active for how long the character can
        yield return new WaitForSeconds(currentShieldDuration);

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
