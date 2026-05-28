using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue =  maxHealth;
        slider.value = maxHealth;
    }
    public void SetCurrentHealth(float currentHealth)
    {
        slider.value = currentHealth;
    }
    
    
}
