using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoatSelector : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI selectText;
    
    [Header("Boat Models In Menu")]
    [SerializeField] private GameObject[] boatModels;
    [SerializeField] private PlayerData[] playerData;
    [SerializeField] private WeaponData[] weaponData;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider shieldBar;
    [SerializeField] private Slider damageBar;
    [SerializeField] private Slider fireRateBar;
    
    [SerializeField] private float maxHealth = 200f;
    [SerializeField] private float maxShieldDuration = 25f ;
    [SerializeField] private float maxDamage = 30f;
    [SerializeField] private float maxFireRate = 5f;

    public int selectedIndex { get; private set; }

    private void OnEnable()
    {
        ShowBoat(0);
    }

    public void NextBoat()
    {
        selectedIndex++;

        if (selectedIndex >= boatModels.Length)
        {
            selectedIndex = 0;
        }

        ShowBoat(selectedIndex);
    }

    public void PreviousBoat()
    {
        selectedIndex--;

        if (selectedIndex < 0)
        {
            selectedIndex = boatModels.Length - 1;
        }

        ShowBoat(selectedIndex);
    }

    public void SelectBoat()
    {
        PlayerPrefs.SetInt("SelectedBoatIndex", selectedIndex);
        PlayerPrefs.Save();
        

        Debug.Log("Selected boat index: " + selectedIndex);
    }

    private void ShowBoat(int index)
    {
        for (int i = 0; i < boatModels.Length; i++)
        {
            UpdateStatBars(index);
            boatModels[i].SetActive(i == index);
        }
        // --- NEW UNLOCK LOGIC ---
        // Check how many boats the player has unlocked via the Daily Reward system
        int unlockedAmount = PlayerPrefs.GetInt("UnlockedBoats", 1);
        
        if (selectButton != null)
        {
            // If the current index is less than the unlocked amount, make the button clickable
            if (index < unlockedAmount)
            {
                selectButton.interactable = true;
                selectText.text = "CONFIRM";
            }
            else // Otherwise, the player hasn't unlocked it yet, so disable the button
            {
                selectButton.interactable = false;
                selectText.text = "LOCKED";
            }
        }
    }
    private void UpdateStatBars(int index)
    {
        // Safety check
        if (playerData == null || playerData.Length == 0)
        {
            return;
        }

        if (index < 0 || index >= playerData.Length)
        {
            return;
        }
        
        if (weaponData == null || weaponData.Length == 0)
        {
            return;
        }

        if (index < 0 || index >= weaponData.Length)
        {
            return;
        }

        
        
            
        
        //Player Data
        PlayerData selectedPlayerData = playerData[index];
        float healthPercent = selectedPlayerData.maxHealth / maxHealth;
        float shieldPercent = selectedPlayerData.shieldDuration / maxShieldDuration;

        healthPercent = Mathf.Clamp01(healthPercent);
        shieldPercent = Mathf.Clamp01(shieldPercent);

        if (healthBar != null)
        {
            healthBar.value = healthPercent;
        }

        if (shieldBar != null)
        {
            shieldBar.value = shieldPercent;
        }
        
        //Weapon Data
        WeaponData selectedWeaponData = weaponData[index];
        float damagePercent = selectedWeaponData.projectileDamage / maxDamage;
        float fireRatePercent = 1f - (selectedWeaponData.fireRate / maxFireRate);

        damagePercent = Mathf.Clamp01(damagePercent);
        fireRatePercent = Mathf.Clamp01(fireRatePercent);
        fireRatePercent = Mathf.Max(0.1f, fireRatePercent);

        if (damageBar != null)
        {
            damageBar.value = damagePercent;
        }

        if (fireRateBar != null)
        {
            fireRateBar.value = fireRatePercent;
        }
    }
    public void HideAllBoats()
    {
        if (boatModels == null) return;

        foreach (GameObject boat in boatModels)
        {
            if (boat != null)
            {
                boat.SetActive(false);
            }
        }
    }
}
